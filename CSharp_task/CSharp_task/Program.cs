using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Semaphore[] forks = new Semaphore[5];
        for (int i = 0; i < 5; i++)
        {
            forks[i] = new Semaphore(1, 1);
        }
        Semaphore forkAccess = new Semaphore(4, 4);

        Philosopher[] philosophers = new Philosopher[5];
        for (int i = 0; i < 5; i++)
        {
            philosophers[i] = new Philosopher(i + 1, forks[i], forks[(i + 1) % 5], forkAccess);
            philosophers[i].Start();
        }
    }
}

class Philosopher
{
    private int id;
    private Semaphore leftFork;
    private Semaphore rightFork;
    private Semaphore forkAccess;

    public Philosopher(int id, Semaphore leftFork, Semaphore rightFork, Semaphore forkAccess)
    {
        this.id = id;
        this.leftFork = leftFork;
        this.rightFork = rightFork;
        this.forkAccess = forkAccess;
    }

    public void Start()
    {
        Thread philosopherThread = new Thread(Run);
        philosopherThread.Start();
    }

    private void Run()
    {
        for (int i = 1; i <= 10; i++)
        {
            try
            {
                Console.WriteLine($"Philosopher {id} thinking {i} time");

                if (!forkAccess.WaitOne(0))
                {
                    Console.WriteLine($"Philosopher {id} couldn't get fork access, waiting");
                    continue;
                }

                leftFork.WaitOne();
                Console.WriteLine($"Philosopher {id} took left fork");
                rightFork.WaitOne();
                Console.WriteLine($"Philosopher {id} took right fork");

                Console.WriteLine($"Philosopher {id} eating {i} time");

                rightFork.Release();
                Console.WriteLine($"Philosopher {id} put right fork");
                leftFork.Release();
                Console.WriteLine($"Philosopher {id} put left fork");

                forkAccess.Release();
            }
            catch (ThreadInterruptedException e) { Console.WriteLine(e.StackTrace); }
        }
    }
}
