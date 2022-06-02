# Python

- Check if you are running python from virtual environment: run python, import ```sys```, then ```sys.prefix```, or do ```pip -V``` or ```python -m pip -V```

-  In python your ```.py``` file is regarded as an object, with the classes in it as attributes of the object.
    For a ```.py``` file, ```sys.modules[__name__]``` returns ```<module '__main__' (built-in)>```, with each class inside the file as attributes.

    So, to check if a class exists in a .py file, you can use ```getattr(sys.modules[__name__], yourclassname)``` inside the file

- For timeouting while loops, use ```interruptingcow``` library
- ```Time.sleep``` is dependent on CPU. Any while loop needs ```time.sleep``` if you want to reduce cpu use of while loops
- What was the exception thrown by a ```try```? Use ```traceback``` module
  ```python
    try: 
       ...
    except Exception:
      logger.console(traceback.format_exc())
   ```

    Always implement this on modules/classes (e.g. code that will to be imported),  so that if python is called from robot framework (or any other parent process), you know which line is the error in

-  To print the whole exception (including the parent who called the code), ```use traceback.print_stack()``` instead. The disadvantage is that this is very verbose.
-  Assignment in python i.e. ```a = b``` does not create a copy of ```b``` for reference types; it makes a point to ```b```. If ```b``` is a dict, and ```a[key]=4``` is done, then ```b[key]``` will also be changed to 4. Be careful. use ```copy``` or ```deepcopy``` module to copy.
- Read ```pep8``` for how to write clean code
- ```functools.partial``` instead of ```lambda```, it is an option
- Easier ask for permission than forgiveness (always use ```try except``` instead of ```if```)
- Use ```itertools``` to eliminate multiple loops
- ```sorted(list, key = lambda x:x, reverse=False)``` https://holypython.com/intermediate-python-exercises/exercise-11-python-lambda/
- Note on lists: if ```list2=[1,2,3]``` then
  
  ```list2[0] = 1```
  
  ```list2[0:1] = [1]```  (i.e. it's NOT ```[1,2]```) !
  
  ```list2[0:2] = [1,2]```

  Be careful

 
- Practice enumerable magic in codewars

- utf-8 vs 16 and how unicode works (in jupyter notebook)

- dict comprehension: https://holypython.com/intermediate-python-exercises/exercise-17-python-dict-comprehensions/

- https://www.datacamp.com/community/tutorials/python-dictionary-comprehension?utm_source=adwords_ppc&utm_campaignid=898687156&utm_adgroupid=48947256715&utm_device=c&utm_keyword=&utm_matchtype=b&utm_network=g&utm_adpostion=&utm_creative=255798340456&utm_targetid=aud-299261629574:dsa-473406574715&utm_loc_interest_ms=&utm_loc_physical_ms=1010704&gclid=Cj0KCQjwxJqHBhC4ARIsAChq4asR1eyAnY1fJnPa-skSZSbHxNzetwfZ_dTqhE3bisVHmzHFXyKH4hMaAt9OEALw_wcB
Do the fahrenheit ones

- Note on iterator (i.e. return value of ```zip```, ```filter```, etc.) on how to print things from iterator e.g. the assignment below:
  
  ```result = filter(lambda x:x==3, list)```
gives an iterator stored in ```result```. To get the values you cannot do ```list(result)``` because ```result``` is an iterator. You need to do ```[e for e in result]``` or [*result] (i.e. unpacking)

    Note that once iterated through, the iterator cannot be iterated again (it is exhausted).

- item 14 of 90 effective ways to write python book: how to create a list of class instances, and use map() or sorted() or others'' key=lambda on each instance's attributes