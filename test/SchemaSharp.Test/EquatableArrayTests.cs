namespace SchemaSharp.Tests;

public sealed class EquatableArrayTests
{
    [Test]
    public void Add_WhenArrayHasItems_ShouldReturnNewArrayWithItemAppended()
    {
        // arrange
        var sut = new EquatableArray<int>([1, 2]);

        // act
        var result = sut.Add(3);

        // assert
        var expected = new EquatableArray<int>([1, 2, 3]);
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(result.Array, Is.Not.SameAs(sut.Array));
    }

    [Test]
    public void Add_WhenArrayIsEmpty_ShouldReturnArrayWithItem()
    {
        // arrange
        var sut = EquatableArray<int>.Empty;

        // act
        var result = sut.Add(5);

        // assert
        var expected = new EquatableArray<int>([5]);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Add_WhenArrayIsNull_ShouldReturnArrayWithItem()
    {
        // arrange
        var sut = new EquatableArray<int>(null!);

        // act
        var result = sut.Add(10);

        // assert
        var expected = new EquatableArray<int>([10]);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Array_WhenInitializedWithDefaultOperator_ShouldNotBeNull()
    {
        // act
        EquatableArray<TypeGeneratorConfiguration> sut = default;

        // assert
        Assert.That(sut.Array, Is.Not.Null);
    }

    [Test]
    public void Equals_WhenArraysHaveSameElements_ShouldReturnTrue()
    {
        // arrange
        var left = new EquatableArray<int>([1, 2]);
        var right = new EquatableArray<int>([1, 2]);

        // act
        var equalsResult = left.Equals(right);

        // assert
        Assert.That(equalsResult, Is.True);
    }

    [Test]
    public void OperatorEquals_WhenArraysHaveSameElements_ShouldReturnTrue()
    {
        // arrange
        var left = new EquatableArray<int>([1, 2]);
        var right = new EquatableArray<int>([1, 2]);

        // act
        var equalsResult = left == right;

        // assert
        Assert.That(equalsResult, Is.True);
    }

    [Test]
    public void Equals_WhenArraysHaveDifferentElements_ShouldReturnFalse()
    {
        // arrange
        var left = new EquatableArray<int>([1, 2]);
        var right = new EquatableArray<int>([2, 1]);

        // act
        var equalsResult = left.Equals(right);

        // assert
        Assert.That(equalsResult, Is.False);
    }

    [Test]
    public void OperatorNotEquals_WhenArraysHaveDifferentElements_ShouldReturnTrue()
    {
        // arrange
        var left = new EquatableArray<int>([1, 2]);
        var right = new EquatableArray<int>([2, 1]);

        // act
        var notEqualsResult = left != right;

        // assert
        Assert.That(notEqualsResult, Is.True);
    }

    [Test]
    public void Equals_WhenObjectIsOtherType_ShouldReturnFalse()
    {
        // arrange
        var sut = new EquatableArray<int>([1, 2]);

        // act
        var result = sut.Equals(new object());

        // assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_WhenObjectIsEqualBoxedEquatableArray_ShouldReturnTrue()
    {
        // arrange
        var sut = new EquatableArray<int>([1, 2]);
        object boxed = new EquatableArray<int>([1, 2]);

        // act
        var result = sut.Equals(boxed);

        // assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void GetHashCode_WhenArraysHaveSameElements_ShouldReturnSameValue()
    {
        // arrange
        var left = new EquatableArray<int>([1, 2]);
        var right = new EquatableArray<int>([1, 2]);

        // act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // assert
        Assert.That(leftHash, Is.EqualTo(rightHash));
    }

    [Test]
    public void Count_WhenArrayHasElements_ShouldReturnLength()
    {
        // arrange
        var sut = new EquatableArray<int>([1, 2, 3]);

        // act
        var result = sut.Count;

        // assert
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public void GetEnumerator_WhenIterated_ShouldReturnAllItems()
    {
        // arrange
        var sut = new EquatableArray<int>([1, 2, 3]);

        // act
        var result = sut.ToArray();

        // assert
        Assert.That(result, Is.EqualTo([1, 2, 3]));
    }
}