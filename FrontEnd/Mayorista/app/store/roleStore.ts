import { create } from 'zustand';

const API_URL = 'http://localhost:5295/api/Roles';

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
        const res = await fetch(API_URL);
        const data = await res.json();
        set({ roles: data });
    },

    fetchPermisos: async () => {
        const res = await fetch(`${API_URL}/permisos`);
        const data = await res.json();
        set({ permisos: data });
    },

    buscarRoles: async (nombre: string) => {
        const res = await fetch(`${API_URL}/buscar?nombre=${nombre}`);
        const data = await res.json();
        set({ roles: data });
    },

    deleteRol: async (id: number) => {
        await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
        set((state) => ({ roles: state.roles.filter(r => r.idRol !== id) }));
    }
}));