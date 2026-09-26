import { create } from 'zustand';
import { persist } from 'zustand/middleware'; 

interface AuthState {
  token: string | null;
  usuario: string | null;
  rol: string | null;
  permisos: string[]; // <-- AGREGAMOS ESTO
  login: (token: string, usuario: string, rol: string, permisos: string[]) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
  hasPermiso: (permiso: string) => boolean; // <-- HERRAMIENTA MAGICA
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      usuario: null,
      rol: null,
      permisos: [], // Inicializamos vacío

      // Actualizamos el login para recibir los permisos
      login: (token, usuario, rol, permisos) => set({ token, usuario, rol, permisos }),
      
      logout: () => set({ token: null, usuario: null, rol: null, permisos: [] }),

      isAuthenticated: () => get().token !== null,

      // Esta funcion revisa si el usuario tiene el permiso exacto
      hasPermiso: (permiso: string) => get().permisos.includes(permiso),
    }),
    {
      name: 'auth-storage', 
    }
  )
);