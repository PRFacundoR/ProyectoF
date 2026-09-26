import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router';
import { useRoleStore } from '../store/roleStore';

// IMPORTANTE: Asegurate de usar TU puerto real acá
const API_URL = 'http://localhost:5295/api/roles';

export default function RolesForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { permisos, fetchPermisos } = useRoleStore();
  
  const isEditing = Boolean(id);

  const [formData, setFormData] = useState({
    nombre: '',
    descripcion: '',
    permisosIds: [] as number[]
  });

  // Estados para el buscador de permisos
  const [searchPermiso, setSearchPermiso] = useState('');
  const [filteredPermisos, setFilteredPermisos] = useState(permisos);

  // 1. Cargar la lista completa de permisos al entrar
  useEffect(() => {
    if (permisos.length === 0) fetchPermisos();
  }, []);

  // 2. Si estamos editando y los permisos ya cargaron, traemos los datos del rol
  useEffect(() => {
    if (isEditing && permisos.length > 0) {
      fetch(`${API_URL}/${id}`) // Usamos la URL correcta
        .then(res => res.json())
        .then(data => {
          // Cruzamos los nombres que manda la API con los IDs de nuestro store
          const idsSeleccionados = permisos
            .filter(p => data.permisosAsignados.includes(p.nombrePermiso))
            .map(p => p.idPermiso);

          setFormData({
            nombre: data.nombre || '',
            descripcion: data.descripcion || '',
            permisosIds: idsSeleccionados
          });
        })
        .catch(err => console.error("Error al cargar el rol:", err));
    }
  }, [id, isEditing, permisos.length]);

  // 3. Efecto Debounce de 500ms para el buscador de permisos
  useEffect(() => {
    const delay = setTimeout(() => {
      if (searchPermiso.trim() === '') {
        setFilteredPermisos(permisos);
      } else {
        const lowerSearch = searchPermiso.toLowerCase();
        setFilteredPermisos(
          permisos.filter(p => 
            p.nombrePermiso.toLowerCase().includes(lowerSearch) || 
            p.descripcion.toLowerCase().includes(lowerSearch)
          )
        );
      }
    }, 500);

    return () => clearTimeout(delay);
  }, [searchPermiso, permisos]);

  const handleCheckbox = (idPermiso: number) => {
    setFormData(prev => {
      const tienePermiso = prev.permisosIds.includes(idPermiso);
      return {
        ...prev,
        permisosIds: tienePermiso 
          ? prev.permisosIds.filter(p => p !== idPermiso) 
          : [...prev.permisosIds, idPermiso]
      };
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const url = isEditing ? `${API_URL}/${id}` : API_URL;
    const method = isEditing ? 'PUT' : 'POST';

    const response = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    });

    if (response.ok) {
      navigate('/roles');
    } else {
      const errorData = await response.json();
      alert(errorData.mensaje || "Ocurrió un error al guardar.");
    }
  };

  return (
    <div className="p-8 max-w-2xl mx-auto">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">
        {isEditing ? 'Editar Rol' : 'Crear Nuevo Rol'}
      </h1>

      <form onSubmit={handleSubmit} className="bg-white p-6 rounded-xl shadow-md">
        <div className="mb-4">
          <label className="block text-gray-700 font-semibold mb-2">Nombre del Rol</label>
          <input 
            type="text" 
            required
            value={formData.nombre}
            onChange={e => setFormData({...formData, nombre: e.target.value})}
            className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
            placeholder="Ej: Administrador"
          />
        </div>

        <div className="mb-6">
          <label className="block text-gray-700 font-semibold mb-2">Descripción</label>
          <textarea 
            required
            value={formData.descripcion}
            onChange={e => setFormData({...formData, descripcion: e.target.value})}
            className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
            placeholder="Ej: Acceso total al sistema..."
          />
        </div>

        <div className="mb-8">
          <div className="flex justify-between items-end mb-4">
            <label className="block text-gray-700 font-semibold">Permisos Asignados</label>
            
            {/* BUSCADOR DE PERMISOS */}
            <input 
              type="text" 
              placeholder="🔍 Buscar permiso..."
              value={searchPermiso}
              onChange={(e) => setSearchPermiso(e.target.value)}
              className="p-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none w-1/2"
            />
          </div>

          {/* LISTA DE PERMISOS FILTRADA */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3 max-h-60 overflow-y-auto p-2 bg-gray-50 border rounded-lg">
            {filteredPermisos.map(permiso => (
              <label key={permiso.idPermiso} className="flex items-start space-x-3 p-3 border bg-white rounded-lg hover:bg-blue-50 cursor-pointer transition">
                <input 
                  type="checkbox" 
                  checked={formData.permisosIds.includes(permiso.idPermiso)}
                  onChange={() => handleCheckbox(permiso.idPermiso)}
                  className="w-5 h-5 mt-1 text-blue-600 rounded focus:ring-blue-500"
                />
                <div>
                  <span className="block font-medium text-gray-800 text-sm">{permiso.nombrePermiso}</span>
                  <span className="block text-xs text-gray-500 leading-tight mt-1">{permiso.descripcion}</span>
                </div>
              </label>
            ))}
            {filteredPermisos.length === 0 && (
              <p className="col-span-2 text-center text-sm text-gray-500 py-4">No se encontraron permisos.</p>
            )}
          </div>
        </div>

        <div className="flex gap-4">
          <button type="submit" className="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 rounded-lg shadow-md transition">
            {isEditing ? 'Guardar Cambios' : 'Crear Rol'}
          </button>
          <button type="button" onClick={() => navigate('/roles')} className="flex-1 bg-gray-200 hover:bg-gray-300 text-gray-800 font-bold py-3 rounded-lg transition">
            Cancelar
          </button>
        </div>
      </form>
    </div>
  );
}