// This file Connects to  backend (/api/auth/login)
//Stores JWT token in browser
//Provides helper for future authenticated API calls
//Provides logout() for future pages



// ======= Configuration =======
const apiBaseUrl = "https://localhost:7171/api"; // My backend API URL
// =============================

// Save token in localStorage
function saveToken(token) {
    localStorage.setItem("jwtToken", token);
}

// Get token from localStorage
function getToken() {
    return localStorage.getItem("jwtToken");
}

// Remove token (logout)
function logout() {
    localStorage.removeItem("jwtToken");
    window.location.href = "index.html";
}

// Perform login request
async function login(email, password) {
    try {
        const response = await fetch(`${apiBaseUrl}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) return false;

        const data = await response.json();
       if (data.token) {  
    saveToken(data.token);
    return true;
}

        return false;
    } catch (error) {
        console.error("Login error:", error);
        return false;
    }
}

// Authenticated GET request
async function authGet(endpoint) {
    const token = getToken();
    const response = await fetch(`${apiBaseUrl}${endpoint}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    if (response.status === 401) {
        alert("Unauthorized. Please log in again.");
        logout();
        return null;
    }

    return await response.json();
}


// Decode JWT token to get user roles
function parseJwt(token) {
  if (!token) return null;
  const base64Url = token.split('.')[1];
  if (!base64Url) return null;

  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  try {
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(c =>
      '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
    ).join(''));
    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
}

// a function to check if user is admin
function isAdmin() {
  const token = getToken();
  const payload = parseJwt(token);
  if (!payload) return false;

  // Possible claim keys where role may be stored
  const roleClaimKeys = [
    "role",
    "roles",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
  ];

  for (const key of roleClaimKeys) {
    const roles = payload[key];
    if (roles) {
      if (Array.isArray(roles) && roles.includes("Admin")) return true;
      if (typeof roles === "string" && roles === "Admin") return true;
    }
  }

  return false;
}


// get current adopter ID
function getCurrentUserId() {
  const token = getToken();
  if (!token) return null;
  const payload = parseJwt(token);
  // Typical claim for user id is 'nameid' or 'sub'
  return payload?.nameid || payload?.sub || null;
}

// Assuming you store user ID in JWT token as 'sub' or 'nameid'
function getUserIdFromToken() {
  const token = getToken();
  if (!token) return null;

  const payload = parseJwt(token);
  return payload ? payload.sub || payload.nameid || null : null;
}
async function getAdopterProfile() {
  try {
    const response = await fetch(`${apiBaseUrl}/adopters/me`, {
      headers: {
        "Authorization": `Bearer ${getToken()}`
      }
    });

    if (response.ok) {
      return await response.json();
    } else {
      alert("Failed to load profile.");
      return null;
    }
  } catch (err) {
    alert("Error loading profile: " + err.message);
    return null;
  }
}
