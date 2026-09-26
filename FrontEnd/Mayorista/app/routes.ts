import { type RouteConfig, index,route } from "@react-router/dev/routes";

export default [
    
    
    index("routes/login.tsx"),
// aca van las rutas de la app, por ejemplo:
/*
  route("dashboard", "routes/dashboard.tsx"),
  route("productos", "routes/productos.tsx"),
  route("carrito", "routes/carrito.tsx"),
*/

  route("roles", "routes/roles.tsx"), // Panel de Roles
 // route("roles/nuevo", "routes/rolesForm.tsx"), // Crear Rol
  //route("roles/editar/:id", "routes/rolesForm.tsx"), // Editar Rol (Mismo componente)
   // Le agregamos un objeto { id: "..." } al final para que sean únicas internamente
  route("roles/nuevo", "routes/rolesForm.tsx", { id: "crear-rol" }), 
  route("roles/editar/:id", "routes/rolesForm.tsx", { id: "editar-rol" }), 

] satisfies RouteConfig;
