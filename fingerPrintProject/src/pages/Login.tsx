import { useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import * as Api from '../services/api'
import './Login.css'
import AmpmLogo from '../Resources/Logo_Ampm 400x134.png'
import { MdFingerprint } from 'react-icons/md'

export default function LoginPage() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [showPass, setShowPass] = useState(false)
  const [remember, setRemember] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const navigate = useNavigate()

  // Nota: lógica de expiración removida — no mostramos alerta basada en flag de expiración.

  async function handleLogin(e: FormEvent) {
    e.preventDefault()
    if (!username || !password) {
      setError('Ingresa usuario y contraseña')
      return
    }

    if (Api.API_ENABLED) {
      try {
        setLoading(true)
        setError(null)
        // Usar directamente endpoint de token para autenticación
        const res = await Api.getToken(username, password)
        if (!res.isSuccess) {
          const backendMsg = res.message || ''
          const msgLower = backendMsg.toLowerCase()
          const isAuthError = msgLower.includes('invalid') || msgLower.includes('unauthorized') || msgLower.includes('not found') || msgLower.includes('credentials') || msgLower.includes('password')
          setError(isAuthError ? 'Usuario o contraseña incorrectos' : backendMsg || 'Usuario o contraseña incorrectos')
          setLoading(false)
          return
        }
        if (res.token) {
          // Almacenamiento original: guardar siempre en localStorage
          localStorage.setItem('fp_token', res.token)
        } else {
        setError(res.message || 'Login inválido')
          setLoading(false)
          return
        }
        setLoading(false)
        navigate('/employees')
        return
      } catch (err) {
        console.error(err)
        setError('No fue posible iniciar sesión contra la API. Revisa la configuración y CORS/SSL.')
        setLoading(false)
        return
      }
    }

  setError('API no configurada. Necesitas definir VITE_API_URL para obtener token.')
  }

  return (
    <div className="login-gradient">
      <div className="login-shell">
        <div className="login-left">
            <div className="finger-box">
              <img src={AmpmLogo} alt="AMPM logo" className="ampm-logo" />
            </div>
        </div>
        <div className="login-right">
          <div className="brand">
              <h1><MdFingerprint className="brand-fingerprint" /> FingerPrint</h1>
            <p className="subtitle">Bienvenido, inicia sesión para continuar</p>
          </div>
          <form onSubmit={handleLogin} className="login-form">
            <label className="field">
              <span>Usuario</span>
              <input
                type="text"
                value={username}
                 onChange={e => { setUsername(e.target.value); setError(null) }}
                placeholder="Usuario"
                disabled={loading}
                required
              />
            </label>
            <label className="field">
              <span>Contraseña</span>
              <div className="password-wrapper">
                <input
                  type={showPass ? 'text' : 'password'}
                  value={password}
                   onChange={e => { setPassword(e.target.value); setError(null) }}
                  disabled={loading}
                  required
                />
                <button
                  type="button"
                  className="toggle-pass"
                  onClick={() => setShowPass(s => !s)}
                  aria-label="Mostrar u ocultar contraseña"
                >
                  {showPass ? '🙈' : '👁️'}
                </button>
              </div>
            </label>
            <div className="row-between">
              <label className="remember">
                <input
                  type="checkbox"
                  checked={remember}
                  onChange={e => setRemember(e.target.checked)}
                  disabled={loading}
                />
                <span>Recordarme</span>
              </label>
              <a href="#" className="link-small">¿Olvidaste tu contraseña?</a>
            </div>
            {error && <div className="form-error" role="alert">{error}</div>}
            <button type="submit" className="btn-primary" disabled={loading}>
              {loading ? 'Ingresando...' : 'Ingresar'}
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}
