using Microsoft.EntityFrameworkCore;
using Sabemi.WebhookApi.Models;

namespace Sabemi.WebhookApi.Data;

public class SabemiDbContext : DbContext
{
    public SabemiDbContext(DbContextOptions<SabemiDbContext> options) : base(options) { }

    public DbSet<EventoBruto> EventosBrutos => Set<EventoBruto>();
    public DbSet<StatusContrato> StatusContratos => Set<StatusContrato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventoBruto>(entity =>
        {
            entity.ToTable("eventos_brutos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdTransacao).HasColumnName("id_transacao");
            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("numeric(15,2)");
            entity.Property(e => e.DataPagamento).HasColumnName("data_pagamento");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.PayloadBruto).HasColumnName("payload_bruto").HasColumnType("jsonb");
            entity.Property(e => e.RecebidoEm).HasColumnName("recebido_em");

            entity.HasIndex(e => e.IdTransacao).IsUnique();
        });

        modelBuilder.Entity<StatusContrato>(entity =>
        {
            entity.ToTable("status_contrato");
            entity.HasKey(e => e.IdContrato);

            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.StatusAtual).HasColumnName("status_atual");
            entity.Property(e => e.UltimaAtualizacao).HasColumnName("ultima_atualizacao");
            entity.Property(e => e.UltimoIdTransacao).HasColumnName("ultimo_id_transacao");
        });
    }
}
