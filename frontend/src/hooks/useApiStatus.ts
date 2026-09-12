import { useEffect, useState } from 'react'
import { checkHealth } from '@/services/healthService'

export type ApiConnectionStatus = 'checking' | 'online' | 'offline'

const DEFAULT_POLL_INTERVAL_MS = 30_000

/**
 * Verifica periodicamente se a API está respondendo, usada pelo indicador
 * de conexão no header. Começa em "checking" e nunca fica "presa" nele:
 * a primeira resposta (sucesso ou falha) já resolve para online/offline.
 */
export function useApiStatus(pollIntervalMs = DEFAULT_POLL_INTERVAL_MS): ApiConnectionStatus {
  const [status, setStatus] = useState<ApiConnectionStatus>('checking')

  useEffect(() => {
    let cancelled = false

    async function ping() {
      try {
        await checkHealth()
        if (!cancelled) setStatus('online')
      } catch {
        if (!cancelled) setStatus('offline')
      }
    }

    ping()
    const interval = setInterval(ping, pollIntervalMs)

    return () => {
      cancelled = true
      clearInterval(interval)
    }
  }, [pollIntervalMs])

  return status
}
