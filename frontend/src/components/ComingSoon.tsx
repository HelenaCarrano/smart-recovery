/**
 * Placeholder temporário para páginas ainda não implementadas nesta etapa.
 * Cada uma será substituída por conteúdo real nas próximas etapas do plano.
 */
export function ComingSoon({ page }: { page: string }) {
  return (
    <div className="flex min-h-[60vh] items-center justify-center">
      <div className="rounded-xl border border-dashed border-slate-300 bg-white px-10 py-8 text-center">
        <p className="text-sm font-medium text-brand-600">{page}</p>
        <p className="mt-1 text-sm text-slate-500">Conteúdo desta página chega na próxima etapa.</p>
      </div>
    </div>
  )
}
