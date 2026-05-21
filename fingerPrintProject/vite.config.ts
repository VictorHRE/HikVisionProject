import { defineConfig, loadEnv } from 'vite'
import path from 'path'

export default defineConfig(({ mode }) => {
  // Cargar variables de entorno para usar VITE_API_URL en config de dev
  const env = loadEnv(mode, process.cwd(), '')
  const VITE_API_URL = env.VITE_API_URL || ''

  // Proxy: mapear /api/* al origin del backend para evitar CORS y problemas con HTTPS
  const proxy: Record<string, { target: string; changeOrigin: boolean; secure: boolean }> = {}
  if (VITE_API_URL) {
    try {
      const u = new URL(VITE_API_URL)
      proxy['/api'] = {
        target: u.origin,
        changeOrigin: true,
        // secure false permite certificados autofirmados en dev
        secure: false,
        // no rewrite por defecto: 
        // /api/authentication/login -> https://host:port/api/authentication/login
      }
    } catch {
      // ignore invalid URL
    }
  }

  return {
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src'),
      },
    },
    server: {
      proxy,
    },
  }
})
