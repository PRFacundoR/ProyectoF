import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import { useRoleStore } from '../store/roleStore';

export default function RolesPanel() {
  const navigate = useNavigate();
  const { roles, fetchRoles, buscarRoles, deleteRol } = useRoleStore();
  const [searchTerm, setSearchTerm] = useState('');

  // Efecto Debounce: Espera 500ms antes de buscar
  useEffect(() => {
    const delayDebounceFn = setTimeout(() => {
      if (searchTerm) {
        buscarRoles(searchTerm);
      } else {
        fetchRoles();
      }
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [searchTerm]);

  return (
    <div className="p-8 max-w-5xl mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">Panel de Roles</h1>
        <button 
          onClick={() => navigate('/roles/nuevo')}
          className="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded-lg shadow-md transition"
        >
          + Nuevo Rol
        </button>
      </div>

      <input
        type="text"
        placeholder="🔍 Buscar rol por nombre..."
        className="w-full mb-6 p-3 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-blue-500 outline-none"
        onChange={(e) => setSearchTerm(e.target.value)}
      />

      <div className="bg-white rounded-xl shadow-md overflow-hidden">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="bg-gray-100 text-gray-600 border-b">
              <th className="p-4">Nombre</th>
              <th className="p-4">Permisos</th>
              <th className="p-4 text-center">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {roles.map((rol) => (
              <tr key={rol.idRol} className="border-b hover:bg-gray-50 transition">
                <td className="p-4 font-semibold text-gray-800">{rol.nombre}</td>
                <td className="p-4">
                  <div className="flex flex-wrap gap-2">
                    {rol.permisosAsignados?.map((p: string) => (
                      <span key={p} className="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full">
                        {p}
                      </span>
                    ))}
                  </div>
                </td>
                <td className="p-4 flex justify-center gap-3">
                  <button 
                    onClick={() => navigate(`/roles/editar/${rol.idRol}`)}
                    className="text-blue-500 hover:text-blue-700 font-medium transition"
                  >
                    ✏️ Editar
                  </button>
                  <button 
                    onClick={() => {
                      if(window.confirm('¿Seguro que deseas borrar este rol?')) deleteRol(rol.idRol);
                    }}
                    className="text-red-500 hover:text-red-700 font-medium transition"
                  >
                    🗑️ Borrar
                  </button>
                </td>
              </tr>
            ))}
            {roles.length === 0 && (
              <tr><td colSpan={4} className="p-6 text-center text-gray-500">No se encontraron roles.</td></tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}