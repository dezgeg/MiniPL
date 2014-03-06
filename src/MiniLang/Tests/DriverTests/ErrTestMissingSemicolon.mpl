// Testing a missing semicolon:

var foo : int := 42 // <- missing ';' expected here
print bar; // <- this line should cause an error about undeclared variable 'bar', which means that this line wasn't ignored.
print foo; // <- this line shouldn't cause an error