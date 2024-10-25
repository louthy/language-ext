Prisms are lenses that allow you to look inside a structure to access its value and 
then subsequently update that value.  But because we don't want to do mutation, the 
updaters create new versions of the underlying structure.

The difference between lenses and prisms is that a prism returns an optional value for
its getter.  
