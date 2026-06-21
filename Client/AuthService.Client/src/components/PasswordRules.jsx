export default function PasswordRules({ password }) {
  const checks = {
    length: password.length >= 8,
    uppercase: /[A-Z]/.test(password),
    number: /\d/.test(password),
  };

  const passed = Object.values(checks).filter(Boolean).length;

  function rule(valid, text) {
    return (
      <li
        className={`text-sm flex items-center gap-2 ${valid ? "text-green-600" : "text-gray-500"}`}
      >
        <span>{valid ? "✔" : "•"}</span>
        {text}
      </li>
    );
  }

  function strengthColor() {
    if (passed === 0) return "bg-gray-200";
    if (passed === 1) return "bg-red-500";
    if (passed === 2) return "bg-yellow-400";
    return "bg-green-500";
  }

  function strengthWidth() {
    return `${(passed / 3) * 100}%`;
  }

  return (
    <div className="mb-4">
      {/* Strength bar */}
      <div className="h-2 w-full bg-gray-200 rounded mb-3">
        <div
          className={`h-2 rounded transition-all ${strengthColor()}`}
          style={{ width: strengthWidth() }}
        />
      </div>

      {/* Rules */}
      <ul className="space-y-1">
        {rule(checks.length, "At least 8 characters")}
        {rule(checks.uppercase, "One uppercase letter")}
        {rule(checks.number, "One number")}
      </ul>
    </div>
  );
}
