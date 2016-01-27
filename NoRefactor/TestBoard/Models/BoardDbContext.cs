using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace TestBoard.Models
{
    /// <summary>
    /// DBの掲示板の情報を格納するモデル
    /// </summary>
    public class BoardEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 第
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 本文
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// 返信
        /// </summary>
        public virtual ICollection<BoardPostEntity> Posts { get; set; }
    }

    /// <summary>
    /// DBの掲示板の返信
    /// </summary>
    public class BoardPostEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Text { get; set; }
    }

    /// <summary>
    /// 掲示板まわりのDBアクセス
    /// </summary>
    public class BoardDbContext : DbContext
    {
        /// <summary>
        /// 生成
        /// </summary>
        public BoardDbContext()
            : base( "DefaultConnection" )
        {
        }

        /// <summary>
        /// DBの掲示板情報を参照
        /// </summary>
        public virtual DbSet<BoardEntity> Boards { get; set; }
    }
}