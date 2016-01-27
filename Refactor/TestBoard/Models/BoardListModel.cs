using System.Collections.Generic;
using System.Linq;

namespace TestBoard.Models
{
    /// <summary>
    /// 掲示板の一覧時のモデル
    /// </summary>
    public class BoardListModel
    {
        /// <summary>
        /// 一覧
        /// </summary>
        public List<BoardEntity> Boards { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public BoardListModel(BoardDbRepository db)
        {
            Boards = db.GetAll();
        }
    }
}