// testing a '=' mistyped as '0'
var foo : int := 42;
assert foo 0 42; // this shouldn't be parsed as 'assert foo;' and cause a type error.