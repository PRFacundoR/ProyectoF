import { useEffect, useState } from "react";
import { Navigate, Outlet, useNavigate } from "react-router";
import { useAuthStore } from "../store/authStore";

export default function ProtectedLayout() {
  // 1. Traemos la función de logout y los datos del usuario para mostrarlos
  const { token, usuario, rol, logout } = useAuthStore();
  const navigate = useNavigate();
  
  const [isHydrated, setIsHydrated] = useState(false);

  useEffect(() => {
    setIsHydrated(true);
  }, []);

  // Función para cerrar sesión
  const handleLogout = () => {
    if (window.confirm("¿Estás seguro de que deseas cerrar sesión?")) {
      logout(); // Esto borra el token del Zustand y del LocalStorage
      navigate("/", { replace: true }); // Lo mandamos de un patadón al Login
    }
  };

  if (!isHydrated) {
    return null;
  }

  if (!token) {
    return <Navigate to="/" replace />;
  }

  return (
    <div className="min-h-screen bg-gray-100 flex flex-col">
      
      {/* NAVBAR GLOBAL (Aparecerá arriba en todas las pantallas protegidas) */}
      <header className="bg-blue-900 text-white p-4 shadow-md flex justify-between items-center">
        <div className="font-bold text-xl tracking-wide">
          🏢 Sistema Mayorista
        </div>
        
        <div className="flex items-center gap-6">
          {/* Muestra quién está logueado */}
          <div className="text-right hidden sm:block">
            <p className="font-semibold text-sm">{usuario}</p>
            <p className="text-blue-300 text-xs font-medium uppercase tracking-wider">{rol}</p>
          </div>
          
          {/* Botón de Cerrar Sesión */}
          <button 
            onClick={handleLogout}
            className="bg-red-500 hover:bg-red-600 text-white text-sm px-4 py-2 rounded-lg font-bold transition shadow-lg flex items-center gap-2"
          >
            <span>🚪</span> Cerrar Sesión
          </button>
        </div>
      </header>

      {/* CONTENIDO DE LA PANTALLA ACTUAL (Ej: El panel de roles se dibujará acá adentro) */}
      <main className="flex-1 p-4">
        <Outlet />
      </main>

    </div>
  );
}