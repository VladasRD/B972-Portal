using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Box.CMS.Models;

namespace Box.CMS.Data {
    

    public class CMSContext : DbContext {

        public CMSContext(DbContextOptions<CMSContext> options) : base(options) {
            //Database.SetInitializer<CMSContext>(new CMSContextInitializer());
            //Database.CommandTimeout = 900;            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<ContentHead>()
                .HasOne(c => c.Data)
                .WithOne()                
                .HasForeignKey<ContentData>(d => d.ContentUId)
                .OnDelete(DeleteBehavior.Cascade);
                

            modelBuilder.Entity<ContentHead>()                
                .HasOne<ContentCommentCount>(c => c.CommentsCount)
                .WithOne()
                .HasForeignKey<ContentCommentCount>(o => o.ContentUId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContentHead>()
                .HasOne<ContentShareCount>(c => c.ShareCount)
                .WithOne()
                .HasForeignKey<ContentShareCount>(s => s.ContentUId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContentHead>()
                .HasOne<ContentPageViewCount>(c => c.PageViewCount)
                .WithOne()
                .HasForeignKey<ContentPageViewCount>(p => p.ContentUId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContentHead>()
                .HasOne<ContentCustomInfo>(c => c.CustomInfo)
                .WithOne()
                .HasForeignKey<ContentCustomInfo>(i => i.ContentUId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<File>()
                .HasOne(f => f.Data)
                .WithOne()
                .HasForeignKey<FileData>(f => f.FileUId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContentTag>()
            .HasKey(tag => new { tag.ContentUId, tag.Tag });

            modelBuilder.Entity<CrossLink>()
            .HasKey(cross => new { cross.ContentUId, cross.PageArea });
        }

        public DbSet<ContentHead> ContentHeads { get; set; }
        public DbSet<ContentData> ContentDatas { get; set; }
        public DbSet<ContentTag> ContentTags { get; set; }
        public DbSet<CrossLink> CrossLinks { get; set; }

        public DbSet<ContentComment> ContentComments { get; set; }
        public DbSet<ContentCommentCount> ContentCommentCounts { get; set; }

        public DbSet<ContentShareCount> ContentSharesCounts { get; set; }
        public DbSet<ContentPageViewCount> ContentPageViewCounts { get; set; }

        public DbSet<ContentCustomInfo> ContentCustomInfos { get; set; }
        

        public DbSet<File> Files { get; set; }
        public DbSet<FileData> FileData { get; set; }


        internal void ApplyCollectionValues<T>(ICollection<T> oldCollection, ICollection<T> newCollection, Func<T, T, bool> predicate) where T: class {
            if (oldCollection == null)
                oldCollection = new List<T>();
            if(newCollection==null)
                newCollection = new List<T>();
            var removed = oldCollection.Where(o => !newCollection.Any(n => predicate(n, o))).ToArray();
            var added = newCollection.Where(n => !oldCollection.Any(o => predicate(n, o))).ToArray();

            foreach (T r in removed)                
                Set<T>().Remove(r);

            foreach (T a in added)
                Set<T>().Add(a);
        }
    }
}
