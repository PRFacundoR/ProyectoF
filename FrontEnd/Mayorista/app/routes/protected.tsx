import { Navigate, Outlet } from "react-router";
import { useAuthStore } from "../store/authStore";

export default function ProtectedLayout() {
  const { token } = useAuthStore();

  if (!token) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}