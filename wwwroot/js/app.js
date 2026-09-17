const EMAIL_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
const ROLE_CLAIM = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

let currentUser = null; // { email, role(s) }
let currentPostId = null;
let currentPostAuthorEmail = null;

// ---------- helpers ----------

function decodeJwt(token) {
  const payloadPart = token.split(".")[1];
  const normalized = payloadPart.replace(/-/g, "+").replace(/_/g, "/");
  const padded = normalized + "===".slice((normalized.length + 3) % 4);
  return JSON.parse(atob(padded));
}

function hasRole(roleName) {
  if (!currentUser || !currentUser.role) return false;
  const role = currentUser.role;
  return Array.isArray(role) ? role.includes(roleName) : role === roleName;
}

function getToken() {
  return sessionStorage.getItem("token");
}

async function api(path, method = "GET", body = null) {
  const headers = { "Content-Type": "application/json" };
  const token = getToken();
  if (token) headers["Authorization"] = "Bearer " + token;

  const response = await fetch(path, {
    method,
    headers,
    body: body ? JSON.stringify(body) : null
  });

  let data = null;
  const text = await response.text();
  if (text) {
    try { data = JSON.parse(text); } catch { data = text; }
  }

  return { ok: response.ok, status: response.status, data };
}

async function apiUpload(path, formData) {
  const headers = {};
  const token = getToken();
  if (token) headers["Authorization"] = "Bearer " + token;

  const response = await fetch(path, { method: "POST", headers, body: formData });

  let data = null;
  const text = await response.text();
  if (text) {
    try { data = JSON.parse(text); } catch { data = text; }
  }

  return { ok: response.ok, status: response.status, data };
}

function showMessage(text, isError) {
  const el = document.getElementById("message");
  el.textContent = text;
  el.className = "message " + (isError ? "err" : "ok");
  el.classList.remove("hidden");
  window.scrollTo({ top: 0, behavior: "smooth" });
  clearTimeout(showMessage._t);
  showMessage._t = setTimeout(() => el.classList.add("hidden"), 6000);
}

function errorMessage(data, fallback) {
  if (data && typeof data === "object" && data.message) return data.message;
  if (Array.isArray(data)) return data.map(e => e.errorMessage).join(" ");
  return fallback;
}

function switchView(viewId) {
  document.querySelectorAll(".view").forEach(v => v.classList.add("hidden"));
  document.getElementById(viewId).classList.remove("hidden");
}

function updateNav() {
  const nav = document.getElementById("nav");
  nav.innerHTML = "";

  if (currentUser) {
    const info = document.createElement("span");
    info.className = "session-info";
    info.textContent = `${currentUser.email} (${Array.isArray(currentUser.role) ? currentUser.role.join(", ") : currentUser.role})`;
    nav.appendChild(info);

    const postsBtn = document.createElement("button");
    postsBtn.textContent = "Publicaciones";
    postsBtn.onclick = () => { switchView("view-posts"); loadPosts(); };
    nav.appendChild(postsBtn);

    if (hasRole("Administrator")) {
      const adminBtn = document.createElement("button");
      adminBtn.textContent = "Admin";
      adminBtn.onclick = () => { switchView("view-admin"); loadAdminUsers(); };
      nav.appendChild(adminBtn);
    }

    const logoutBtn = document.createElement("button");
    logoutBtn.textContent = "Cerrar sesión";
    logoutBtn.onclick = logout;
    nav.appendChild(logoutBtn);
  } else {
    const postsBtn = document.createElement("button");
    postsBtn.textContent = "Publicaciones";
    postsBtn.onclick = () => { switchView("view-posts"); loadPosts(); };
    nav.appendChild(postsBtn);

    const loginBtn = document.createElement("button");
    loginBtn.textContent = "Iniciar sesión";
    loginBtn.onclick = () => switchView("view-login");
    nav.appendChild(loginBtn);

    const registerBtn = document.createElement("button");
    registerBtn.textContent = "Registrarme";
    registerBtn.onclick = () => switchView("view-register");
    nav.appendChild(registerBtn);
  }

  document.getElementById("new-post-box").classList.toggle("hidden", !currentUser);
}

function logout() {
  sessionStorage.removeItem("token");
  currentUser = null;
  updateNav();
  switchView("view-posts");
  loadPosts();
}

function restoreSession() {
  const token = getToken();
  if (!token) return;
  try {
    const payload = decodeJwt(token);
    if (payload.exp && Date.now() >= payload.exp * 1000) {
      sessionStorage.removeItem("token");
      return;
    }
    currentUser = { email: payload[EMAIL_CLAIM], role: payload[ROLE_CLAIM] };
  } catch {
    sessionStorage.removeItem("token");
  }
}

// ---------- registro ----------

document.getElementById("form-register").addEventListener("submit", async (e) => {
  e.preventDefault();
  const email = document.getElementById("reg-email").value;
  const password = document.getElementById("reg-password").value;
  const passwordConfirm = document.getElementById("reg-password-confirm").value;

  const res = await api("/api/Users/register", "POST", { email, password, passwordConfirm });
  if (res.ok) {
    showMessage("Cuenta creada. Confirma tu correo para poder iniciar sesión.", false);
    document.getElementById("confirm-box").classList.remove("hidden");
  } else {
    showMessage(errorMessage(res.data, "No se pudo completar el registro."), true);
  }
});

document.getElementById("form-confirm").addEventListener("submit", async (e) => {
  e.preventDefault();
  const token = document.getElementById("confirm-token").value.trim();

  const res = await api(`/api/Users/confirm-email?token=${encodeURIComponent(token)}`, "GET");
  if (res.ok) {
    showMessage("Correo confirmado. Ya puedes iniciar sesión.", false);
    document.getElementById("confirm-box").classList.add("hidden");
    document.getElementById("form-register").reset();
    switchView("view-login");
  } else {
    showMessage(errorMessage(res.data, "El token de confirmación no es válido."), true);
  }
});

// ---------- login ----------

document.getElementById("form-login").addEventListener("submit", async (e) => {
  e.preventDefault();
  const email = document.getElementById("login-email").value;
  const password = document.getElementById("login-password").value;

  const res = await api("/api/Login/authenticate", "POST", { email, password });
  if (res.ok) {
    sessionStorage.setItem("token", res.data.token);
    restoreSession();
    updateNav();
    document.getElementById("form-login").reset();
    showMessage(`Sesión iniciada como ${currentUser.email}.`, false);
    switchView("view-posts");
    loadPosts();
  } else {
    showMessage(errorMessage(res.data, "No se pudo iniciar sesión."), true);
  }
});

// ---------- posts ----------

async function loadPosts() {
  const res = await api("/api/Posts", "GET");
  const list = document.getElementById("posts-list");
  list.innerHTML = "";

  if (!res.ok) {
    showMessage(errorMessage(res.data, "No se pudieron cargar las publicaciones."), true);
    return;
  }

  if (res.data.length === 0) {
    list.innerHTML = '<p class="hint">Todavía no hay publicaciones.</p>';
    return;
  }

  res.data.forEach(post => {
    const item = document.createElement("div");
    item.className = "post-item";
    item.innerHTML = `
      <h3>${escapeHtml(post.title)}</h3>
      <p class="post-meta">Por ${escapeHtml(post.authorEmail)} &middot; ${new Date(post.createdOn).toLocaleString()}</p>
    `;
    item.onclick = () => openPostDetail(post.id);
    list.appendChild(item);
  });
}

function escapeHtml(str) {
  const div = document.createElement("div");
  div.textContent = str ?? "";
  return div.innerHTML;
}

document.getElementById("form-post").addEventListener("submit", async (e) => {
  e.preventDefault();
  const title = document.getElementById("post-title").value;
  const content = document.getElementById("post-content").value;

  const res = await api("/api/Posts", "POST", { title, content });
  if (res.ok) {
    document.getElementById("form-post").reset();
    showMessage("Publicación creada.", false);
    loadPosts();
  } else {
    showMessage(errorMessage(res.data, "No se pudo crear la publicación."), true);
  }
});

// ---------- detalle de post ----------

async function openPostDetail(id) {
  const res = await api(`/api/Posts/${id}`, "GET");
  if (!res.ok) {
    showMessage(errorMessage(res.data, "No se pudo cargar la publicación."), true);
    return;
  }

  currentPostId = id;
  currentPostAuthorEmail = res.data.authorEmail;

  const card = document.getElementById("post-detail-card");
  card.innerHTML = `
    <h2>${escapeHtml(res.data.title)}</h2>
    <p class="post-meta">Por ${escapeHtml(res.data.authorEmail)} &middot; ${new Date(res.data.createdOn).toLocaleString()}</p>
    <p>${escapeHtml(res.data.content)}</p>
  `;

  const isOwner = currentUser && currentUser.email === res.data.authorEmail;
  const isAdmin = hasRole("Administrator");
  document.getElementById("post-owner-actions").classList.toggle("hidden", !isOwner && !isAdmin);
  document.getElementById("btn-edit-post").classList.toggle("hidden", !isOwner);
  document.getElementById("btn-delete-post").classList.toggle("hidden", !isOwner && !isAdmin);
  document.getElementById("edit-post-box").classList.add("hidden");

  document.getElementById("edit-post-title").value = res.data.title;
  document.getElementById("edit-post-content").value = res.data.content;

  document.getElementById("form-comment").classList.toggle("hidden", !currentUser);
  document.getElementById("comment-login-hint").classList.toggle("hidden", !!currentUser);

  document.getElementById("pictures-box").classList.toggle("hidden", !currentUser);
  document.getElementById("form-picture").dataset.isOwner = isOwner ? "true" : "false";
  await loadPictures(id);

  await loadComments(id);
  switchView("view-post-detail");
}

document.getElementById("btn-back-to-posts").addEventListener("click", () => {
  switchView("view-posts");
  loadPosts();
});

document.getElementById("btn-edit-post").addEventListener("click", () => {
  document.getElementById("edit-post-box").classList.remove("hidden");
});

document.getElementById("form-edit-post").addEventListener("submit", async (e) => {
  e.preventDefault();
  const title = document.getElementById("edit-post-title").value;
  const content = document.getElementById("edit-post-content").value;

  const res = await api(`/api/Posts/${currentPostId}`, "PUT", { title, content });
  if (res.ok) {
    showMessage("Publicación actualizada.", false);
    openPostDetail(currentPostId);
  } else {
    showMessage(errorMessage(res.data, "No se pudo actualizar la publicación."), true);
  }
});

document.getElementById("btn-delete-post").addEventListener("click", async () => {
  if (!confirm("¿Eliminar esta publicación? Esta acción no se puede deshacer.")) return;

  const res = await api(`/api/Posts/${currentPostId}`, "DELETE");
  if (res.ok || res.status === 204) {
    showMessage("Publicación eliminada.", false);
    switchView("view-posts");
    loadPosts();
  } else {
    showMessage(errorMessage(res.data, "No se pudo eliminar la publicación."), true);
  }
});

// ---------- imágenes ----------

async function loadPictures(postId) {
  const res = await api(`/api/posts/${postId}/pictures`, "GET");
  const list = document.getElementById("pictures-list");
  list.innerHTML = "";

  if (!res.ok) return;

  res.data.forEach(pic => {
    const item = document.createElement("div");
    item.className = "picture-item";
    item.innerHTML = `
      <img src="${pic.url}" alt="${escapeHtml(pic.fileOriginalName)}">
      <div class="picture-name">${escapeHtml(pic.fileName)}</div>
    `;
    list.appendChild(item);
  });

  const form = document.getElementById("form-picture");
  form.classList.toggle("hidden", form.dataset.isOwner !== "true" || res.data.length >= 5);
}

document.getElementById("form-picture").addEventListener("submit", async (e) => {
  e.preventDefault();
  const fileInput = document.getElementById("picture-file");
  const file = fileInput.files[0];
  if (!file) return;

  const formData = new FormData();
  formData.append("file", file);

  const res = await apiUpload(`/api/posts/${currentPostId}/pictures`, formData);
  if (res.ok) {
    fileInput.value = "";
    showMessage("Imagen subida.", false);
    loadPictures(currentPostId);
  } else {
    showMessage(errorMessage(res.data, "No se pudo subir la imagen."), true);
  }
});

// ---------- comentarios ----------

async function loadComments(postId) {
  const res = await api(`/api/posts/${postId}/comments`, "GET");
  const list = document.getElementById("comments-list");
  list.innerHTML = "";

  if (!res.ok) return;

  if (res.data.length === 0) {
    list.innerHTML = '<p class="hint">Sin comentarios todavía.</p>';
    return;
  }

  const isAdmin = hasRole("Administrator");

  res.data.forEach(comment => {
    const isOwner = currentUser && currentUser.email === comment.authorEmail;
    const item = document.createElement("div");
    item.className = "comment-item";

    let actionsHtml = "";
    if (isOwner || isAdmin) {
      actionsHtml = '<div class="comment-actions">';
      if (isOwner) actionsHtml += `<button data-action="edit" data-id="${comment.id}">Editar</button>`;
      actionsHtml += `<button data-action="delete" data-id="${comment.id}" class="danger">Eliminar</button>`;
      actionsHtml += "</div>";
    }

    item.innerHTML = `
      <p class="comment-meta">${escapeHtml(comment.authorEmail)} &middot; ${new Date(comment.createdOn).toLocaleString()}</p>
      <p class="comment-text">${escapeHtml(comment.content)}</p>
      ${actionsHtml}
    `;
    list.appendChild(item);
  });
}

document.getElementById("comments-list").addEventListener("click", async (e) => {
  const button = e.target.closest("button[data-action]");
  if (!button) return;

  const commentId = button.dataset.id;
  const item = button.closest(".comment-item");

  if (button.dataset.action === "delete") {
    if (!confirm("¿Eliminar este comentario?")) return;
    const res = await api(`/api/posts/${currentPostId}/comments/${commentId}`, "DELETE");
    if (res.ok || res.status === 204) {
      showMessage("Comentario eliminado.", false);
      loadComments(currentPostId);
    } else {
      showMessage(errorMessage(res.data, "No se pudo eliminar el comentario."), true);
    }
    return;
  }

  if (button.dataset.action === "edit") {
    const textEl = item.querySelector(".comment-text");
    const currentText = textEl.textContent;

    const editBox = document.createElement("div");
    editBox.className = "comment-edit-box";
    editBox.innerHTML = `
      <textarea rows="2">${escapeHtml(currentText)}</textarea>
      <div class="comment-edit-buttons">
        <button data-action="save" data-id="${commentId}">Guardar</button>
        <button data-action="cancel" class="cancel">Cancelar</button>
      </div>
    `;
    textEl.replaceWith(editBox);
    return;
  }

  if (button.dataset.action === "cancel") {
    loadComments(currentPostId);
    return;
  }

  if (button.dataset.action === "save") {
    const textarea = item.querySelector("textarea");
    const content = textarea.value;

    const res = await api(`/api/posts/${currentPostId}/comments/${commentId}`, "PUT", { content });
    if (res.ok) {
      showMessage("Comentario actualizado.", false);
      loadComments(currentPostId);
    } else {
      showMessage(errorMessage(res.data, "No se pudo actualizar el comentario."), true);
    }
  }
});

document.getElementById("form-comment").addEventListener("submit", async (e) => {
  e.preventDefault();
  const content = document.getElementById("comment-content").value;

  const res = await api(`/api/posts/${currentPostId}/comments`, "POST", { content });
  if (res.ok) {
    document.getElementById("comment-content").value = "";
    showMessage("Comentario publicado.", false);
    loadComments(currentPostId);
  } else {
    showMessage(errorMessage(res.data, "No se pudo publicar el comentario."), true);
  }
});

// ---------- admin ----------

document.getElementById("form-admin").addEventListener("submit", async (e) => {
  e.preventDefault();
  const email = document.getElementById("admin-email").value;
  const password = document.getElementById("admin-password").value;
  const passwordConfirm = document.getElementById("admin-password-confirm").value;

  const res = await api("/api/Users/create-admin", "POST", { email, password, passwordConfirm });
  if (res.ok) {
    showMessage(`Administrador ${res.data.email} creado. Ya puede iniciar sesión sin confirmar correo.`, false);
    document.getElementById("form-admin").reset();
    loadAdminUsers();
  } else {
    showMessage(errorMessage(res.data, "No se pudo crear el administrador."), true);
  }
});

async function loadAdminUsers() {
  const res = await api("/api/Users/all", "GET");
  const list = document.getElementById("admin-users-list");
  list.innerHTML = "";

  if (!res.ok) {
    showMessage(errorMessage(res.data, "No se pudo cargar la lista de usuarios."), true);
    return;
  }

  res.data.forEach(user => {
    const item = document.createElement("div");
    item.className = "user-item";
    item.innerHTML = `
      <span>#${user.id} &middot; ${escapeHtml(user.email)}</span>
      <button data-id="${user.id}">Eliminar</button>
    `;
    list.appendChild(item);
  });
}

document.getElementById("admin-users-list").addEventListener("click", async (e) => {
  const button = e.target.closest("button[data-id]");
  if (!button) return;

  if (!confirm("¿Eliminar este usuario? Esta acción no se puede deshacer.")) return;

  const res = await api(`/api/Users/${button.dataset.id}`, "DELETE");
  if (res.ok || res.status === 204) {
    showMessage("Usuario eliminado.", false);
    loadAdminUsers();
  } else {
    showMessage(errorMessage(res.data, "No se pudo eliminar el usuario."), true);
  }
});

// ---------- arranque ----------

restoreSession();
updateNav();
switchView("view-posts");
loadPosts();
