namespace WindowsApp.Models.Class
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<FileModel> Files { get; set; }
        public DateTime DateTime { get; set; }
        public string Device { get; set; }
        public int Status { get; set; }

        public Project()
        {
            Files = new List<FileModel>();
        }

        public Project(string name, string directoryPath)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Files = new List<FileModel>();
        }
    }
    public class Either<TLeft, TRight>{
        public TLeft? Left { get; }
        public TRight? Right { get; }
        public bool IsLeft { get; }
        public bool IsRight => !IsLeft;

        private Either(TLeft left)
        {
            Left = left;
            IsLeft = true;
        }

        private Either(TRight right)
        {
            Right = right;
            IsLeft = false;
        }

        public static Either<TLeft, TRight> FromLeft(TLeft left) => new(left);
        public static Either<TLeft, TRight> FromRight(TRight right) => new(right);
    }
}