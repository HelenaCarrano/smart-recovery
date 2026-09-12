interface ErrorStateProps {
  message: string
  onRetry?: () => void
}

/** Usado tanto para erros de negócio quanto para "API indisponível" (mensagem já vem tratada de lib/errors). */
export function ErrorState({ message, onRetry }: ErrorStateProps) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 rounded-xl border border-status-declined-border bg-status-declined-bg px-6 py-10 text-center">
      <p className="text-sm font-medium text-status-declined">{message}</p>
      {onRetry && (
        <button
          type="button"
          onClick={onRetry}
          className="rounded-lg border border-status-declined-border bg-white px-3 py-1.5 text-xs font-medium text-status-declined transition-colors hover:bg-status-declined-bg"
        >
          Tentar novamente
        </button>
      )}
    </div>
  )
}
