using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF.Domain;

namespace WebApplication2.Data.EF;

public class ApplicationContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
        
    }
    
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatRole> ChatRoles { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<ChatMessageImageLink> ImageLinks { get; set; }
    public DbSet<ChatUser> ChatUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(DateTime)
                                 || p.ClrType == typeof(DateTime?))
                     .Where(p => p.GetColumnType() == null))
        {
            property.SetColumnType("timestamp without time zone");
        }
        
        builder.Entity<User>()
            .HasMany(u => u.ChatUsers)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);
        
        builder.Entity<User>()
            .HasMany(u => u.InvitedByUser)
            .WithOne(e => e.Inviter)
            .HasForeignKey(e => e.InviterId);
        
        builder.Entity<Chat>()
            .HasMany(u => u.ChatUsers)
            .WithOne(e => e.Chat)
            .HasForeignKey(e => e.ChatId);
        
        builder.Entity<Chat>()
            .HasMany(u => u.Messages)
            .WithOne(e => e.Chat)
            .HasForeignKey(e => e.ChatId);
        
        builder.Entity<ChatMessage>()
            .HasMany(u => u.MessageImageLinks)
            .WithOne(e => e.ChatMessage)
            .HasForeignKey(e => e.ChatMessageId);
        
        builder.Entity<ChatMessage>()
            .HasOne(u => u.Owner)
            .WithMany(e => e.ChatMessages)
            .HasForeignKey(e => e.OwnerId);
        
        builder.Entity<Image>()
            .HasMany(u => u.MessageImageLinks)
            .WithOne(e => e.Image)
            .HasForeignKey(e => e.ImageId);
        
        base.OnModelCreating(builder);
    }
}