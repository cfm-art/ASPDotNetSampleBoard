using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBoard.Controllers;
using System.Web.Mvc;
using TestBoard.Models;
using System.Data.Entity;
using Moq;
using System.Linq;
using System.Collections.Generic;

namespace TestBoard.Tests.Controllers
{
    /// <summary>
    /// 掲示板回りのテスト
    /// </summary>
    [TestClass]
    public class BoardControllerTest
    {
        /// <summary>
        /// 掲示板一覧のテスト
        /// </summary>
        [TestMethod]
        public void Index()
        {
            // DBのモックを用意する
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            var originalData = new List<BoardEntity> {
                new BoardEntity { Id = 1, Title = "A", Text = "a" },
                new BoardEntity { Id = 2, Title = "B", Text = "b" },
                new BoardEntity { Id = 3, Title = "C", Text = "c" },
            };
            var data = originalData.AsQueryable();

            // 各メソッドの返り値をモックに差し替える
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.Provider ).Returns( data.Provider );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.Expression ).Returns( data.Expression );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.ElementType ).Returns( data.ElementType );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.GetEnumerator() ).Returns( data.GetEnumerator() );

            mockcontext.Setup( m => m.Boards ).Returns( mockset.Object );

            var controller = new BoardController( mockcontext.Object );
            ViewResult result = controller.Index() as ViewResult;

            //  モデルのデータがちゃんとDBのデータを取得出来ているか検証
            var model = result.Model as BoardListModel;
            Assert.AreSame( originalData[0], model.Boards[0] );
            Assert.AreSame( originalData[1], model.Boards[1] );
            Assert.AreSame( originalData[2], model.Boards[2] );

            Assert.IsNotNull( result );
        }

        /// <summary>
        /// 掲示板詳細のテスト
        /// </summary>
        [TestMethod]
        public void Show()
        {
            // DBのモックを用意する
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            // 掲示板の情報
            var postOriginalData = new List<BoardPostEntity> {
                new BoardPostEntity { Text = "投稿1" },
                new BoardPostEntity { Text = "投稿2" }
            };

            // レスの情報
            var originalData = new List<BoardEntity> {
                new BoardEntity { Id = 1, Title = "A", Text = "a", Posts = postOriginalData },
            };
            var data = originalData.AsQueryable();

            // 各メソッドの返り値をモックに差し替える
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.Provider ).Returns( data.Provider );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.Expression ).Returns( data.Expression );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.ElementType ).Returns( data.ElementType );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.GetEnumerator() ).Returns( data.GetEnumerator() );

            mockcontext.Setup( m => m.Boards ).Returns( mockset.Object );

            var controller = new BoardController( mockcontext.Object );
            ViewResult result = controller.Show( 1 ) as ViewResult;

            //  モデルのデータがちゃんとDBのデータを取得出来ているか検証
            var model = result.Model as BoardEntity;
            Assert.AreSame( originalData[0], model );
            Assert.AreSame( postOriginalData[0], model.Posts.ToArray()[0] );
            Assert.AreSame( postOriginalData[1], model.Posts.ToArray()[1] );

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
            // モック用意
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            // ダミーデータの生成
            var model = new BoardCreateModel {
                Title = "題名",
                Text = "本文"
            };

            // Addの返り値フック
            var dummy = new BoardEntity { Id = 1, Title = model.Title, Text = model.Text };
            mockset.As<IDbSet<BoardEntity>>().Setup( m => m.Add( It.IsAny<BoardEntity>() ) ).Returns( dummy );

            mockcontext.Setup( m => m.Boards ).Returns( mockset.Object );

            var controller = new BoardController(mockcontext.Object);
            var result = controller.Create(model) as RedirectResult;
            Assert.IsNotNull( result );

            // Addが呼ばれたかチェック
            mockset.Verify( m => m.Add( It.Is<BoardEntity>( o => o.Title == model.Title && o.Text == model.Text ) ), Times.Once );

            // SaveChangesがよばれたかチェック
            mockcontext.Verify( m => m.SaveChanges(), Times.Once );

            Assert.AreEqual( result.Url, "/Board/Show/1" );
        }

        /// <summary>
        /// 返信の投稿のテスト
        /// </summary>
        [TestMethod]
        public void PostResponse()
        {
            // DBのモックを用意する
            var mockposts = new Mock<ICollection<BoardPostEntity>>();
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            var originalData = new List<BoardEntity> {
                new BoardEntity { Id = 1, Title = "A", Text = "a", Posts = mockposts.Object },
            };
            var data = originalData.AsQueryable();

            // 各メソッドの返り値をモックに差し替える
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.Provider ).Returns( data.Provider );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.Expression ).Returns( data.Expression );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.ElementType ).Returns( data.ElementType );
            mockset.As<IQueryable<BoardEntity>>().Setup( m => m.GetEnumerator() ).Returns( data.GetEnumerator() );

            mockcontext.Setup( m => m.Boards ).Returns( mockset.Object );

            var postData = new BoardPostModel { Text = "投稿内容" };

            var controller = new BoardController(mockcontext.Object);
            var result = controller.PostResponse(1, postData ) as RedirectResult;

            //  データの追加がちゃんとされているかチェック
            mockposts.Verify( m => m.Add( It.Is<BoardPostEntity>( o => o.Text == postData.Text ) ), Times.Once );
            mockcontext.Verify( m => m.SaveChanges(), Times.Once );

            Assert.AreEqual( result.Url, "/Board/Show/1" );
        }
    }
}
