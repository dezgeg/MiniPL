     print "Give a number"; 
     var n : int;
     read n;
     var f : int := 1;
     var i : int;
     for i in 1..n do 
         f := f * i;
     end for;
     print "The result is: ";
     print f; 