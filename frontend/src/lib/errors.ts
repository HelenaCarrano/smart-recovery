import axios from 'axios'

interface ApiErrorBody {
  message?: string
}

/**
 * Traduz um erro do Axios em uma mensagem apresentável. Distingue "API fora do ar"
 * (sem resposta nenhuma — servidor não está rodando ou CORS bloqueou) de um erro
 * de negócio retornado pela API (que já vem com uma mensagem clara do backend).
 */
export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    if (!error.response) {
      return 'Não foi possível conectar à API. Verifique se o backend está em execução.'
    }

    const body = error.response.data as ApiErrorBody | undefined
    return body?.message ?? `Erro ${error.response.status} ao consultar a API.`
  }

  return 'Ocorreu um erro inesperado ao carregar os dados.'
}
