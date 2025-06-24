using System;
using System.Collections.Generic;

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }//요소가 현재 힙의 어느 위치에 있는지 추적, 정렬 시 부모나 자식 인덱스를 계산하는 데 사용됨.
}

public class PriorityQueue<T> where T : IHeapItem<T>//HeapIndex는 int타입을 저장하기위하여 구현해놓은 함수.
{
    private List<T> items = new();
    //ex) items = [A, B, C] Count == 3 이상태에서 D를 추가

    public int Count => items.Count;

    public void Enqueue(T item)// 항목을 items 리스트 끝에 추가하고,SortUp(item) 호출로 올바른 위치로 올림
    {
        item.HeapIndex = items.Count; // A,B,C 3개 
        items.Add(item); // D가 리스트의 인덱스 3에 들어감.
        SortUp(item); //정렬.
    }

    public T Dequeue()//항목을 items 리스트에서 제거하고,SortDown(item) 호출로 올바른 위치로 내림
    {
        if (items.Count == 0)
            throw new InvalidOperationException("큐가 비어 있습니다.");

        T firstItem = items[0];// items[0]는 항상 우선순위가 가장 높은 항목
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
        while ((parentIndex = (item.HeapIndex - 1) / 2) >= 0) //(item.HJeapIndex-1)/2 는 부모노드를 구하는 공식.
        {
            T parent = items[parentIndex];
            if (item.CompareTo(parent) < 0)//-1,0,1을 반환하는 CompareTo 메서드로 비교하여 현재 노드가 부모 노드보다 우선순위가 높으면 Swap
                Swap(item, parent);
            else
                break;
        }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int leftChild = item.HeapIndex * 2 + 1;// 왼쪽 자식 노드 인덱스는 현재 노드 인덱스 * 2 + 1
            int rightChild = item.HeapIndex * 2 + 2;// 오른쪽 자식 노드 인덱스는 현재 노드 인덱스 * 2 + 2
            int swapIndex = item.HeapIndex;

            if (leftChild < Count && items[leftChild].CompareTo(items[swapIndex]) < 0)
                // 왼쪽 자식 노드가 현재 노드보다 우선순위가 높으면 swapIndex를 왼쪽 자식 인덱스로 설정
                swapIndex = leftChild;

            if (rightChild < Count && items[rightChild].CompareTo(items[swapIndex]) < 0)
                // 오른쪽 자식 노드가 현재 노드보다 우선순위가 높으면 swapIndex를 오른쪽 자식 인덱스로 설정
                swapIndex = rightChild;

            if (swapIndex != item.HeapIndex)
                // 현재 노드와 swapIndex에 해당하는 노드를 비교하여 우선순위가 낮은 노드를 위로 올림
                Swap(item, items[swapIndex]);
            else
                break;
        }
    }

    private void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        (a.HeapIndex, b.HeapIndex) = (b.HeapIndex, a.HeapIndex);//C# 7.0부터 지원되는 튜플을 이용한 스왑
    }
}