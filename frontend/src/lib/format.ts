const currencyFormatter = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })
const percentFormatter = new Intl.NumberFormat('pt-BR', { style: 'percent', maximumFractionDigits: 1 })
const dateTimeFormatter = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' })

export function formatCurrency(value: number): string {
  return currencyFormatter.format(value)
}

/** Espera uma fração (0.42), não um percentual pronto (42). */
export function formatPercent(value: number): string {
  return percentFormatter.format(value)
}

export function formatDateTime(iso: string): string {
  return dateTimeFormatter.format(new Date(iso))
}
