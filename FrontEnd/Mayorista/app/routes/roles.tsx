import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import { useRoleStore } from '../store/roleStore';
import { useAuthStore } from '../store/authStore';

export default function RolesPanel() {
  const navigate = useNavigate();
  const { roles, fetchRoles, buscarRoles, deleteRol } = useRoleStore();
  const [searchTerm, setSearchTerm] = useState('');
  const { hasPermiso, rol: miRolActual } = useAuthStore();

  // Efecto Debounce: Espera 500ms antes de buscar en la API
  useEffect(() => {
    const delayDebounceFn = setTimeout(() => {
      if (searchTerm) {
        buscarRoles(searchTerm);
      } else {
        fetchRoles();
      }
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [searchTerm, buscarRoles, fetchRoles]);

  return (
    <div className="p-8 max-w-6xl mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">Panel de Roles</h1>
        
        {hasPermiso('CREAR_ROLES') && (
          <button 
            onClick={() => navigate('/roles/nuevo')}
            className="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded-lg shadow-md transition"
          >
            + Nuevo Rol
          </button>
        )}
      </div>

      <input
        type="text"
        placeholder="🔍 Buscar rol por nombre..."
        className="w-full mb-6 p-3 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-blue-500 outline-none"
        onChange={(e) => setSearchTerm(e.target.value)}
      />

      {/* CONTENEDOR DE LA TABLA CON SCROLL MÁGICO */}
      <div className="bg-white rounded-xl shadow-md overflow-hidden border border-gray-200">
        
        {/* MAGIA 1: max-h-[60vh] limita el alto de la tabla al 60% de la pantalla. Si hay muchos roles, aparece el scroll vertical */}
        <div className="max-h-[60vh] overflow-y-auto">
          <table className="w-full text-left border-collapse">
            
            {/* MAGIA 2: sticky top-0 hace que la cabecera se quede "pegada" al hacer scroll hacia abajo */}
            <thead className="sticky top-0 bg-gray-100 z-10 shadow-sm">
              <tr className="text-gray-600 border-b">
                <th className="p-4 w-1/4">Nombre</th>
                <th className="p-4 w-2/4">Permisos</th>
                <th className="p-4 w-1/4 text-center">Acciones</th>
              </tr>
            </thead>
            
            <tbody>
              {roles.map((rol) => (
                <tr key={rol.idRol} className="border-b hover:bg-gray-50 transition">
                  <td className="p-4 font-semibold text-gray-800 align-top">{rol.nombre}</td>
                  
                  {/* CELDA DE PERMISOS */}
                  <td className="p-4 align-top">
                    
                    {/* MAGIA 3: max-h-24 (aprox 3 renglones) y overflow-y-auto. 
                        Si tiene más de 8/10 permisos, aparece un scroll chiquito SOLO adentro de esta celda */}
                    <div className="flex flex-wrap gap-2 max-h-28 overflow-y-auto pr-2 custom-scrollbar">
                      {rol.permisosAsignados?.map((p: string) => (
                        <span key={p} className="bg-blue-100 text-blue-800 text-[10px] font-bold px-2 py-1 rounded-full whitespace-nowrap">
                          {p}
                        </span>
                      ))}
                    </div>

                  </td>

                  <td className="p-4 flex justify-center gap-3 align-top">
                    {hasPermiso('ACTUALIZAR_ROLES') && rol.nombre !== miRolActual && rol.nombre !== 'Admin' && (
                      <button 
                        onClick={() => navigate(`/roles/editar/${rol.idRol}`)}
                        className="text-blue-500 hover:text-blue-700 font-medium transition"
                      >
                        ✏️ Editar
                      </button>
                    )}
                    {hasPermiso('BORRAR_ROLES') && rol.nombre !== miRolActual && rol.nombre !== 'Admin' && (
                      <button 
                        onClick={() => {
                          if(window.confirm('¿Seguro que deseas borrar este rol?')) deleteRol(rol.idRol);
                        }}
                        className="text-red-500 hover:text-red-700 font-medium transition"
                      >
                        🗑️ Borrar
                      </button>
                    )}
                  </td>
                </tr>
              ))}
              {roles.length === 0 && (
                <tr><td colSpan={3} className="p-6 text-center text-gray-500">No se encontraron roles.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}