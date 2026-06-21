import { useEffect, useState } from "react";
import { getUserDetails } from "../api/authService";
import Navbar from "../components/Navbar";

export default function UserDetailsPage() {
  const [user, setUser] = useState(null);

  useEffect(() => {
    async function loadUser() {
      const data = await getUserDetails();
      setUser(data);
    }

    loadUser();
  }, []);

  if (!user) return <p className="text-center mt-10">Loading...</p>;

  return (
    <div className="min-h-screen bg-slate-100">
      <Navbar />

      <div className="flex justify-center mt-10">
        <div className="bg-white p-6 rounded-lg shadow w-96">
          <h2 className="text-xl font-semibold mb-4 text-blue-600">
            Welcome {user.firstName}
          </h2>

          <p>
            <strong>First Name:</strong> {user.firstName}
          </p>

          <p>
            <strong>Last Name:</strong> {user.lastName}
          </p>

          <p>
            <strong>Email:</strong> {user.email}
          </p>
        </div>
      </div>
    </div>
  );
}
