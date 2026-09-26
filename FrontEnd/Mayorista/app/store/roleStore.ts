import { create } from 'zustand';
import { useAuthStore } from './authStore';



const API_URL = 'http://localhost:5295/api/Roles';


const getHeaders = () => ({
  'Content-Type': 'application/json',
  'Authorization': `Bearer ${useAuthStore.getState().token}` 
});
interface RoleStore {
    roles: any[];
    permisos: any[];
    fetchRoles: () => Promise<void>;
    fetchPermisos: () => Promise<void>;
    buscarRoles: (nombre: string) => Promise<void>;
    deleteRol: (id: number) => Promise<void>;
}

export const useRoleStore = create<RoleStore>((set) => ({
    roles: [],
    permisos: [],

    fetchRoles: async () => {
        // AGREGAMOS LOS HEADERS ACÁ
        const res = await fetch(API_URL, { headers: getHeaders() });
        if (res.ok) {
            const data = await res.json();
            set({ roles: data });
        }
    },

    fetchPermisos: async () => {
        // AGREGAMOS LOS HEADERS ACÁ
        const res = await fetch(`${API_URL}/permisos`, { headers: getHeaders() });
        if (res.ok) {
            const data = await res.json();
            set({ permisos: data });
        }
    },

    buscarRoles: async (nombre: string) => {
        // AGREGAMOS LOS HEADERS ACÁ
        const res = await fetch(`${API_URL}/buscar?nombre=${nombre}`, { headers: getHeaders() });
        if (res.ok) {
            const data = await res.json();
            set({ roles: data });
        }
    },

    deleteRol: async (id: number) => {
        // AGREGAMOS LOS HEADERS ACÁ
        const res = await fetch(`${API_URL}/${id}`, { method: 'DELETE', headers: getHeaders() });
        if (res.ok) {
            set((state) => ({ roles: state.roles.filter(r => r.idRol !== id) }));
        } else {
            alert("Error al intentar borrar el rol.");
        }
    }
}));