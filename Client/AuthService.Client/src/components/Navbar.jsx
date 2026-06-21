import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";

export default function Navbar() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <div className="w-full bg-blue-600 text-white px-6 py-3 flex justify-between items-center shadow">
      <h1 className="font-semibold text-lg">Auth System</h1>

      <button
        onClick={handleLogout}
        className="bg-yellow-400 text-black px-3 py-1 rounded hover:bg-yellow-300"
      >
        Logout
      </button>
    </div>
  );
}
