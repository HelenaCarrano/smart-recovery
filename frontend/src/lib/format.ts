const currencyFormatter = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })
const percentFormatter = new Intl.NumberFormat('pt-BR', { style: 'percent', maximumFractionDigits: 1 })
const dateTimeFormatter = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' })
const dateFormatter = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short' })

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

/** Para valores já sem componente de hora (ex: DateOnly do backend, "2026-09-12"). */
export function formatDate(isoDate: string): string {
  return dateFormatter.format(new Date(`${isoDate}T00:00:00Z`))
}
