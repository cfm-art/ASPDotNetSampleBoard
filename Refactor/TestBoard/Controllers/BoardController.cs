﻿using System.Linq;
using System.Web.Mvc;
using TestBoard.Models;

namespace TestBoard.Controllers
{
    /// <summary>
    /// 掲示板の参照・変更回りのコントローラ
    /// /Board
    /// </summary>
    public class BoardController : Controller
    {
        /// <summary>
        /// 掲示板のDB回りの処理
        /// </summary>
        private BoardDbRepository db_;

        /// <summary>
        /// DBを指定せずに生成
        /// </summary>
        public BoardController() : this( null )
        {
        }

        /// <summary>
        /// DBを指定して生成
        /// </summary>
        /// <param name="db"></param>
        public BoardController( BoardDbContext db )
        {
            db_ = new BoardDbRepository( db ?? new BoardDbContext() );
        }

        /// <summary>
        /// GET: Board
        /// 掲示板の一覧
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new BoardListModel( db_ );
            return View( model );
        }

        /// <summary>
        /// GET: Board/Show/{ID}
        /// 掲示板の詳細を表示
        /// </summary>
        /// <param name="id">掲示板のID</param>
        /// <returns></returns>
        public ActionResult Show( int id )
        {
            var board = db_.Find( id );
            return View( board );
        }

        /// <summary>
        /// GET: Board/Create
        /// 掲示板の投稿ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Board/Create
        /// 掲示板の投稿
        /// </summary>
        /// <param name="data">投稿内容</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(BoardCreateModel data)
        {
            int result = db_.Add( new BoardEntity {
                Title = data.Title,
                Text = data.Text
            } );
            
            return Redirect("/Board/Show/" + result);
        }

        /// <summary>
        /// POST: Board/PostResponse/{id}
        /// 掲示板に返信を投稿
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PostResponse( int id, BoardPostModel data )
        {
            db_.PostResponse( id, new BoardPostEntity {
                Text = data.Text
            } );
            return Redirect("/Board/Show/" + id);
        }
    }
}