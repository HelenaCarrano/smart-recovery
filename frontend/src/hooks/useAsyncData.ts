import { getErrorMessage } from '@/lib/errors'
import { useCallback, useEffect, useState } from 'react'

export type AsyncState<T> =
  | { status: 'loading'; refetch: () => void }
  | { status: 'error'; error: string; refetch: () => void }
  | { status: 'success'; data: T; refetch: () => void }

type InternalState<T> = { status: 'loading' } | { status: 'error'; error: string } | { status: 'success'; data: T }

/**
 * Primitiva compartilhada por todos os hooks de dados: carrega uma vez ao montar,
 * expõe loading/error/success de forma explícita e um `refetch` para os botões
 * de "tentar novamente" do ErrorState.
 */
export function useAsyncData<T>(fetcher: () => Promise<T>, deps: unknown[]): AsyncState<T> {
  const [state, setState] = useState<InternalState<T>>({ status: 'loading' })

  const load = useCallback(() => {
    setState({ status: 'loading' })
    fetcher()
      .then((data) => setState({ status: 'success', data }))
      .catch((error: unknown) => setState({ status: 'error', error: getErrorMessage(error) }))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps)

  useEffect(() => {
    load()
  }, [load])

  return { ...state, refetch: load } as AsyncState<T>
}
