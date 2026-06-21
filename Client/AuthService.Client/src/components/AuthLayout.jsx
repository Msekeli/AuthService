export default function AuthLayout({ title, children }) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100">
      <div className="w-full max-w-md bg-white p-8 rounded-xl shadow-lg border-t-4 border-yellow-400">
        <h1 className="text-2xl font-semibold mb-6 text-center text-blue-600">
          {title}
        </h1>

        {children}
      </div>
    </div>
  );
}
