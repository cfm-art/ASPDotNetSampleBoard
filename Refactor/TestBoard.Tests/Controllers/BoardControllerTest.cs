using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBoard.Controllers;
using System.Web.Mvc;
using TestBoard.Models;
using System.Data.Entity;
using Moq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace TestBoard.Tests.Controllers
{
    /// <summary>
    /// 掲示板回りのテスト
    /// </summary>
    [TestClass]
    public class BoardControllerTest
    {
        /// <summary>
        /// 返信のFKを追加した版
        /// </summary>
        private class PostEntity
        {
            /// <summary>
            /// FK
            /// </summary>
            public int ForeignKey { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public BoardPostEntity Post { get; set; }
        }

        /// <summary>
        /// モック達
        /// </summary>
        private class Mocks
        {
            /// <summary>
            /// DbContext
            /// </summary>
            public Mock<BoardDbContext> Context { get; private set; }

            /// <summary>
            /// 掲示板Entity
            /// </summary>
            public Mock<DbSet<BoardEntity>> Boards { get; private set; }

            /// <summary>
            /// 掲示板Entityの元
            /// </summary>
            public List<BoardEntity> OriginalBoards { get; private set; }

            /// <summary>
            /// ポストEntitiy
            /// </summary>
            private List<Mock<ICollection<BoardPostEntity>>> Posts { get; set; }

            /// <summary>
            /// ポストEntitiy
            /// </summary>
            private List<List<BoardPostEntity>> OriginalPosts { get; set; }

            /// <summary>
            /// モック生成
            /// </summary>
            /// <param name="boards">掲示板のダミーデータ</param>
            /// <param name="posts">返信のダミーデータ</param>
            /// <param name="addDummy">Addの返却値のダミー</param>
            public Mocks( IEnumerable<BoardEntity> boards = null, IEnumerable<PostEntity> posts = null, BoardEntity addDummy = null )
            {
                var mockset = new Mock<DbSet<BoardEntity>>();
                var mockcontext = new Mock<BoardDbContext>();

                var originalData = new List<BoardEntity>( boards ?? new BoardEntity[0] );

                // リレーションシップの解決
                Posts = new List<Mock<ICollection<BoardPostEntity>>>();
                OriginalPosts = new List<List<BoardPostEntity>>();
                foreach ( var board in originalData ) {
                    var originalPosts = new List<BoardPostEntity>();
                    if ( posts != null ) {
                        originalPosts = (from o in posts where o.ForeignKey == board.Id select o.Post).ToList();
                        board.Posts = originalPosts;
                    } else {
                        var mockposts = new Mock<ICollection<BoardPostEntity>>();
                        board.Posts = mockposts.Object;
                        Posts.Add( mockposts );
                    }
                    OriginalPosts.Add( originalPosts );
                }

                // 各メソッドの返り値をモックに差し替える
                SetupMockLinq( mockset, originalData );

                // Addの返り値フック
                if ( addDummy != null ) {
                    mockset.As<IDbSet<BoardEntity>>().Setup( m => m.Add( It.IsAny<BoardEntity>() ) ).Returns( addDummy );
                }

                mockcontext.Setup( m => m.Boards ).Returns( mockset.Object );

                Boards = mockset;
                Context = mockcontext;
                OriginalBoards = originalData;
            }

            /// <summary>
            /// モックのLinq用メソッドを差し替え
            /// </summary>
            /// <typeparam name="I"></typeparam>
            /// <param name="mock"></param>
            /// <param name="data"></param>
            private static void SetupMockLinq<I>( Mock<DbSet<I>> mock, List<I> data )
                where I : class
            {
                // 各メソッドの返り値をモックに差し替える
                var q = data.AsQueryable();
                mock.As<IQueryable<I>>().Setup( m => m.Provider ).Returns( q.Provider );
                mock.As<IQueryable<I>>().Setup( m => m.Expression ).Returns( q.Expression );
                mock.As<IQueryable<I>>().Setup( m => m.ElementType ).Returns( q.ElementType );
                mock.As<IQueryable<I>>().Setup( m => m.GetEnumerator() ).Returns( q.GetEnumerator() );
            }

            /// <summary>
            /// 返信を取得
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public List<BoardPostEntity> GetOriginalPosts( int index )
            {
                return OriginalPosts[index];
            }

            /// <summary>
            /// 返信を取得
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Mock<ICollection<BoardPostEntity>> GetPosts( int index )
            {
                return Posts[index];
            }
        }

        /// <summary>
        /// 掲示板一覧のテスト
        /// </summary>
        [TestMethod]
        public void Index()
        {
            // DBのモック用意
            var mocks = new Mocks(new List<BoardEntity> {
                new BoardEntity { Id = 1, Title = "A", Text = "a" },
                new BoardEntity { Id = 2, Title = "B", Text = "b" },
                new BoardEntity { Id = 3, Title = "C", Text = "c" },
            }, null );
        
            var controller = new BoardController( mocks.Context.Object );
            ViewResult result = controller.Index() as ViewResult;

            //  モデルのデータがちゃんとDBのデータを取得出来ているか検証
            var model = result.Model as BoardListModel;
            Assert.AreSame( mocks.OriginalBoards[0], model.Boards[0] );
            Assert.AreSame( mocks.OriginalBoards[1], model.Boards[1] );
            Assert.AreSame( mocks.OriginalBoards[2], model.Boards[2] );

            Assert.IsNotNull( result );
        }

        /// <summary>
        /// 掲示板詳細のテスト
        /// </summary>
        [TestMethod]
        public void Show()
        {
            var mocks = new Mocks(
                new List<BoardEntity> {
                    new BoardEntity { Id = 1, Title = "A", Text = "a" },
                },
                new List<PostEntity> {
                    new PostEntity { ForeignKey = 1, Post = new BoardPostEntity { Text = "投稿1" } },
                    new PostEntity { ForeignKey = 1, Post = new BoardPostEntity { Text = "投稿2" } }
                }
            );

            var controller = new BoardController( mocks.Context.Object );
            ViewResult result = controller.Show( 1 ) as ViewResult;

            //  モデルのデータがちゃんとDBのデータを取得出来ているか検証
            var model = result.Model as BoardEntity;
            Assert.AreSame( mocks.OriginalBoards[0], model );
            var left = mocks.GetOriginalPosts( 0 );
            var right = model.Posts.ToArray();
            Assert.AreSame( mocks.GetOriginalPosts( 0 )[0], model.Posts.ToArray()[0] );
            Assert.AreSame( mocks.GetOriginalPosts( 0 )[1], model.Posts.ToArray()[1] );

            Assert.IsNotNull( result );
        }

        /// <summary>
        /// 掲示板の投稿ページのテスト
        /// </summary>
        [TestMethod]
        public void Create()
        {
            // エラーが無いかだけチェック
            var controller = new BoardController();
            var result = controller.Create() as ViewResult;
            Assert.IsNotNull( result );
        }

        /// <summary>
        /// 掲示板の投稿のテスト
        /// </summary>
        [TestMethod]
        public void PostCreate()
        {
            // ダミーデータの生成
            var model = new BoardCreateModel {
                Title = "題名",
                Text = "本文"
            };

            var dummy = new BoardEntity { Id = 1, Title = model.Title, Text = model.Text };
            var mocks = new Mocks( addDummy: dummy );

            var controller = new BoardController(mocks.Context.Object);
            var result = controller.Create(model) as RedirectResult;
            Assert.IsNotNull( result );

            // Addが呼ばれたかチェック
            mocks.Boards.Verify( m => m.Add( It.Is<BoardEntity>( o => o.Title == model.Title && o.Text == model.Text ) ), Times.Once );

            // SaveChangesがよばれたかチェック
            mocks.Context.Verify( m => m.SaveChanges(), Times.Once );

            Assert.AreEqual( result.Url, "/Board/Show/1" );
        }

        /// <summary>
        /// 返信の投稿のテスト
        /// </summary>
        [TestMethod]
        public void PostResponse()
        {
            // DBのモックを用意する
            var mocks = new Mocks(
                new List<BoardEntity> {
                    new BoardEntity { Id = 1, Title = "A", Text = "a" },
                }
            );

            var postData = new BoardPostModel { Text = "投稿内容" };

            var controller = new BoardController(mocks.Context.Object);
            var result = controller.PostResponse(1, postData ) as RedirectResult;

            //  データの追加がちゃんとされているかチェック
            mocks.GetPosts(0).Verify( m => m.Add( It.Is<BoardPostEntity>( o => o.Text == postData.Text ) ), Times.Once );
            mocks.Context.Verify( m => m.SaveChanges(), Times.Once );

            Assert.AreEqual( result.Url, "/Board/Show/1" );
        }
    }
}
