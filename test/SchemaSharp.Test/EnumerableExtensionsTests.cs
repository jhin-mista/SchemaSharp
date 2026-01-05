namespace SchemaSharp.Tests;

public sealed class EnumerableExtensionsTests
{
    [Test]
    public void ToEquatableArray_WhenSourceIsArray_ShouldWrapExistingArray()
    {
        // arrange
        int[] source = [1, 2, 3];

        // act
        var result = source.ToEquatableArray();

        // assert
        Assert.That(result.Array, Is.EquivalentTo(source));
    }

    [Test]
    public void ToEquatableArray_WhenSourceIsEnumerable_ShouldMaterializeSequence()
    {
        // arrange
        var source = Enumerable.Range(1, 3);

        // act
        var result = source.ToEquatableArray();

        // assert
        Assert.That(result, Is.EqualTo([1, 2, 3]));
    }

    [Test]
    public void ToEquatableArray_WhenCancellationRequested_ShouldThrowOperationCanceledException()
    {
        // arrange
        IEnumerable<int> source = Enumerable.Range(0, 3);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // act
        var invocation = () => source.ToEquatableArray(cancellationTokenSource.Token);

        // assert
        Assert.That(invocation, Throws.InstanceOf<OperationCanceledException>());
    }
}