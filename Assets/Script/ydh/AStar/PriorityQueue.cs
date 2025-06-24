using System;
using System.Collections.Generic;

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }//��Ұ� ���� ���� ��� ��ġ�� �ִ��� ����, ���� �� �θ� �ڽ� �ε����� ����ϴ� �� ����.
}

public class PriorityQueue<T> where T : IHeapItem<T>//HeapIndex�� intŸ���� �����ϱ����Ͽ� �����س��� �Լ�.
{
    private List<T> items = new();
    //ex) items = [A, B, C] Count == 3 �̻��¿��� D�� �߰�

    public int Count => items.Count;

    public void Enqueue(T item)// �׸��� items ����Ʈ ���� �߰��ϰ�,SortUp(item) ȣ��� �ùٸ� ��ġ�� �ø�
    {
        item.HeapIndex = items.Count; // A,B,C 3�� 
        items.Add(item); // D�� ����Ʈ�� �ε��� 3�� ��.
        SortUp(item); //����.
    }

    public T Dequeue()//�׸��� items ����Ʈ���� �����ϰ�,SortDown(item) ȣ��� �ùٸ� ��ġ�� ����
    {
        if (items.Count == 0)
            throw new InvalidOperationException("ť�� ��� �ֽ��ϴ�.");

        T firstItem = items[0];// items[0]�� �׻� �켱������ ���� ���� �׸�
        int lastIndex = items.Count - 1;

        if (lastIndex == 0)
        {
            items.Clear();
            return firstItem;
        }

        items[0] = items[lastIndex];
        items[0].HeapIndex = 0;
        items.RemoveAt(lastIndex);

        SortDown(items[0]);
        return firstItem;
    }

    private void SortUp(T item)
    {
        int parentIndex;
        while ((parentIndex = (item.HeapIndex - 1) / 2) >= 0) //(item.HJeapIndex-1)/2 �� �θ��带 ���ϴ� ����.
        {
            T parent = items[parentIndex];
            if (item.CompareTo(parent) < 0)//-1,0,1�� ��ȯ�ϴ� CompareTo �޼���� ���Ͽ� ���� ��尡 �θ� ��庸�� �켱������ ������ Swap
                Swap(item, parent);
            else
                break;
        }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int leftChild = item.HeapIndex * 2 + 1;// ���� �ڽ� ��� �ε����� ���� ��� �ε��� * 2 + 1
            int rightChild = item.HeapIndex * 2 + 2;// ������ �ڽ� ��� �ε����� ���� ��� �ε��� * 2 + 2
            int swapIndex = item.HeapIndex;

            if (leftChild < Count && items[leftChild].CompareTo(items[swapIndex]) < 0)
                // ���� �ڽ� ��尡 ���� ��庸�� �켱������ ������ swapIndex�� ���� �ڽ� �ε����� ����
                swapIndex = leftChild;

            if (rightChild < Count && items[rightChild].CompareTo(items[swapIndex]) < 0)
                // ������ �ڽ� ��尡 ���� ��庸�� �켱������ ������ swapIndex�� ������ �ڽ� �ε����� ����
                swapIndex = rightChild;

            if (swapIndex != item.HeapIndex)
                // ���� ���� swapIndex�� �ش��ϴ� ��带 ���Ͽ� �켱������ ���� ��带 ���� �ø�
                Swap(item, items[swapIndex]);
            else
                break;
        }
    }

    private void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        (a.HeapIndex, b.HeapIndex) = (b.HeapIndex, a.HeapIndex);//C# 7.0���� �����Ǵ� Ʃ���� �̿��� ����
    }
}