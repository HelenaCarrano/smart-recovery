import axios from 'axios'

/**
 * Instância única do Axios usada por todos os services. Centraliza a base URL
 * (via variável de ambiente) e cabeçalhos padrão, para que os componentes nunca
 * precisem conhecer detalhes de transporte HTTP.
 */
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    'Content-Type': 'application/json',
    ...(import.meta.env.VITE_API_KEY ? { 'X-Api-Key': import.meta.env.VITE_API_KEY } : {}),
  },
})
