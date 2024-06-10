using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model;
using Newtonsoft.Json.Linq;

namespace Bomberman.Test
{
    [TestFixture]
    public class ConcurrentFieldMatrixTest
    {
        private ConcurrentFieldMatrix fields = null!;
        private List<Field>[,] matrix = null!;
        private int Size = 10;

        [SetUp]
        public void Setup()
        {
            matrix = new List<Field>[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    matrix[i, j] = new List<Field>();
                }
            }
            fields = new ConcurrentFieldMatrix(matrix);
        }
        [Test]
        public void TestConstructor()
        {
            Assert.That(fields.GetFieldMatrix(), Is.EqualTo(matrix));
        }
        [Test]
        public void TestGetFieldMatrix()
        {
            Assert.That(fields.GetFieldMatrix(), Is.EqualTo(matrix));
        }
        [Test]
        public void TestAddFields()
        {
            Field value = new Chest(true, 2, 2);
            fields.AddFields(2,2,value);           
            Assert.That(fields.GetFieldMatrix()[2, 2][0], Is.EqualTo(value));
            Field value1 = new Chest(true, 2, 2);
            fields.AddFields(2, 2, value1);
            Assert.That(fields.GetFieldMatrix()[2, 2][1], Is.EqualTo(value1));
            Assert.That(fields.GetFieldMatrix()[2, 2][0], Is.Not.EqualTo(value1));
            Field value2 = new Bomb(2,0,0,null!);
            fields.AddFields(0, 0, value2);
            Assert.That(fields.GetFieldMatrix()[0, 0][0], Is.EqualTo(value2));

        }
        [Test]
        public void TestRemoveField()
        {
            Field value = new Chest(true, 2, 2);
            fields.AddFields(2, 2, value);
            Field value1 = new Chest(true, 2, 2);
            fields.AddFields(2, 2, value1);
            Field value2 = new Bomb(2, 0, 0, null!);
            fields.AddFields(0, 0, value2);

            Assert.That(fields.GetFieldMatrix()[2, 2].Count, Is.EqualTo(2));
            fields.RemoveField(2,2,value1);
            Assert.That(fields.GetFieldMatrix()[2, 2].Count, Is.EqualTo(1));
            fields.RemoveField(2, 2, value1);
            Assert.That(fields.GetFieldMatrix()[2, 2].Count, Is.EqualTo(1));
            fields.RemoveField(2, 2, value2);
            Assert.That(fields.GetFieldMatrix()[2, 2].Count, Is.EqualTo(1));
        }

    }   
}
