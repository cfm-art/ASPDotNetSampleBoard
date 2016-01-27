using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestBoard.Models
{
    /// <summary>
    /// DB処理
    /// </summary>
    public class BoardDbRepository
    {
        /// <summary>
        /// DB
        /// </summary>
        private BoardDbContext db_;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public BoardDbRepository( BoardDbContext db )
        {
            db_ = db;
        }

        /// <summary>
        /// 全掲示板取得
        /// </summary>
        /// <returns>全ての掲示板</returns>
        public List<BoardEntity> GetAll()
        {
            return (from o in db_.Boards select o).ToList();
        }

        /// <summary>
        /// IDで検索
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>検索結果</returns>
        public BoardEntity Find( int id )
        {
            return (from o in db_.Boards where o.Id == id select o).DefaultIfEmpty( null ).Single();
        }

        /// <summary>
        /// 掲示板追加
        /// </summary>
        /// <param name="newBoard">追加する掲示板情報</param>
        /// <returns>追加した掲示板のID</returns>
        public int Add( BoardEntity newBoard )
        {
            var result = db_.Boards.Add( newBoard );
            db_.SaveChanges();
            return result != null ? result.Id : 0;
        }

        /// <summary>
        /// 返信を投稿
        /// </summary>
        /// <param name="board"></param>
        /// <param name="data">投稿データ</param>
        public void PostResponse( BoardEntity board, BoardPostEntity data )
        {
            if ( board != null ) {
                board.Posts.Add( data );
                db_.SaveChanges();
            }
        }

        /// <summary>
        /// 返信を投稿
        /// </summary>
        /// <param name="id">掲示板ID</param>
        /// <param name="data">投稿データ</param>
        public void PostResponse( int id, BoardPostEntity data )
        {
            PostResponse( Find( id ), data );
        }
    }
}