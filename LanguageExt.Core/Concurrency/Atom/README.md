Atoms provide a way to manage shared, synchronous, independent state without locks.  You can use them to wrap up immutable data structures
and then atomically update them using the various `Swap` methods, or read them by calling the `Value` property.  

If a conflict is encountered during a `Swap` operation, the operation is re-run using the latest state, and so you should minimise the time 
spent in the `Swap` function, as well as make sure there are no side-effects, otherwise all bets are off.

See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.

### Usage

    record Person(string Name, string Surname);

    // Create a new atom
    var person = Atom(new Person("Paul", "Louth"));

    // Modify it atomically
    person.Swap(p => p with { Surname = $"{p.Name}y" });

    // Take a snapshot of the state of the atom 
    var snapshot = p.Value;