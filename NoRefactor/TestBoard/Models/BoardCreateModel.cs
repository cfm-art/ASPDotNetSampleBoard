namespace TestBoard.Models
{
    /// <summary>
    /// 掲示板投稿用モデル
    /// </summary>
    public class BoardCreateModel
    {
        /// <summary>
        /// 題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 本文
        /// </summary>
        public string Text { get; set; }
    }
}