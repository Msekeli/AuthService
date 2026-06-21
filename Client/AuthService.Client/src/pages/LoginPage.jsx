import { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import { login as loginRequest } from "../api/authService";
import { useAuth } from "../context/useAuth";
import AuthLayout from "../components/AuthLayout";

export default function LoginPage() {
  const navigate = useNavigate();
  const { login, isAuthenticated } = useAuth();

  const [showPassword, setShowPassword] = useState(false);

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (isAuthenticated) {
      navigate("/me");
    }
  }, [isAuthenticated, navigate]);

  function handleChange(e) {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      const response = await loginRequest(formData);

      login(response.token);

      navigate("/me");
    } catch (error) {
      console.error(error);
      setError("Invalid email or password");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <AuthLayout title="Login">
      {error && (
        <p className="text-red-500 text-sm mb-4 text-center">{error}</p>
      )}

      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="text-sm font-medium">Email</label>

          <input
            type="email"
            name="email"
            placeholder="example@email.com"
            value={formData.email}
            onChange={handleChange}
            required
            className="w-full border rounded p-2 focus:ring-2 focus:ring-blue-500 outline-none"
          />
        </div>

        <div className="mb-5">
          <label className="text-sm font-medium">Password</label>

          <div className="relative">
            <input
              type={showPassword ? "text" : "password"}
              name="password"
              placeholder="Enter password"
              value={formData.password}
              onChange={handleChange}
              required
              className="w-full border rounded p-2 pr-10 focus:ring-2 focus:ring-blue-500 outline-none"
            />

            <button
              type="button"
              onClick={() => setShowPassword(!showPassword)}
              className="absolute right-2 top-2 text-sm text-gray-500"
            >
              {showPassword ? "Hide" : "Show"}
            </button>
          </div>
        </div>

        <button
          type="submit"
          disabled={isLoading}
          className="w-full bg-blue-600 text-white p-2 rounded hover:bg-blue-700 disabled:opacity-50"
        >
          {isLoading ? "Signing in..." : "Login"}
        </button>
      </form>

      <p className="text-sm text-center mt-4">
        Don't have an account?{" "}
        <Link to="/register" className="text-yellow-600 font-medium">
          Register
        </Link>
      </p>
    </AuthLayout>
  );
}
