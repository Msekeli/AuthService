import { apiRequest } from "./apiClient";

export async function register(data) {
  return apiRequest("/api/auth/register", {
    method: "POST",
    body: JSON.stringify({
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      password: data.password,
    }),
  });
}

export async function login(data) {
  const response = await apiRequest("/api/auth/login", {
    method: "POST",
    body: JSON.stringify({
      email: data.email,
      password: data.password,
    }),
  });

  // store JWT token
  if (response?.token) {
    localStorage.setItem("token", response.token);
  }

  return response;
}

export function getUserDetails() {
  return apiRequest("/api/auth/me");
}
