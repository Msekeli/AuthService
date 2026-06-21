import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { register as registerRequest } from "../api/authService";
import PasswordRules from "../components/PasswordRules";

export default function RegisterPage() {
  const navigate = useNavigate();

  const [showPassword, setShowPassword] = useState(false);

  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const password = formData.password;

  const passwordChecks = {
    length: password.length >= 8,
    uppercase: /[A-Z]/.test(password),
    number: /\d/.test(password),
  };

  const passwordValid =
    passwordChecks.length && passwordChecks.uppercase && passwordChecks.number;

  const passwordsMatch =
    formData.password && formData.password === formData.confirmPassword;

  const formValid =
    formData.fullName && formData.email && passwordValid && passwordsMatch;

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

    if (!formValid) return;

    setIsLoading(true);

    try {
      const parts = formData.fullName.trim().split(" ");
      const firstName = parts[0];
      const lastName = parts.slice(1).join(" ") || "";

      await registerRequest({
        firstName,
        lastName,
        email: formData.email,
        password: formData.password,
      });

      navigate("/login");
    } catch (error) {
      console.error(error);
      setError(error.message || "Registration failed");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100">
      <form
        onSubmit={handleSubmit}
        className="w-full max-w-md bg-white p-8 rounded-xl shadow-lg border-t-4 border-yellow-400"
      >
        <h1 className="text-2xl font-semibold mb-6 text-center text-blue-600">
          Create Account
        </h1>

        {error && (
          <p className="text-red-500 text-sm mb-4 text-center">{error}</p>
        )}

        {/* FULL NAME */}

        <div className="mb-3">
          <label className="text-sm font-medium">Full Name</label>

          <input
            type="text"
            name="fullName"
            placeholder="John Doe"
            value={formData.fullName}
            onChange={handleChange}
            required
            autoFocus
            className="w-full border rounded p-2 focus:ring-2 focus:ring-blue-500 outline-none"
          />
        </div>

        {/* EMAIL */}

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

        {/* PASSWORD */}

        <div className="mb-3">
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

        {/* PASSWORD RULES */}

        <PasswordRules password={formData.password} />

        {/* CONFIRM PASSWORD */}

        <div className="mb-5">
          <label className="text-sm font-medium">Confirm Password</label>

          <input
            type="password"
            name="confirmPassword"
            placeholder="Confirm password"
            value={formData.confirmPassword}
            onChange={handleChange}
            required
            className="w-full border rounded p-2 focus:ring-2 focus:ring-blue-500 outline-none"
          />

          {formData.confirmPassword && (
            <p
              className={`text-sm mt-1 ${
                passwordsMatch ? "text-green-600" : "text-red-500"
              }`}
            >
              {passwordsMatch ? "✔ Passwords match" : "Passwords do not match"}
            </p>
          )}
        </div>

        {/* REGISTER BUTTON */}

        <button
          type="submit"
          disabled={!formValid || isLoading}
          className="w-full bg-blue-600 text-white p-2 rounded hover:bg-blue-700 disabled:opacity-50"
        >
          {isLoading ? "Creating account..." : "Register"}
        </button>

        <p className="text-sm text-center mt-4">
          Already have an account?{" "}
          <Link to="/login" className="text-yellow-600 font-medium">
            Login
          </Link>
        </p>
      </form>
    </div>
  );
}
