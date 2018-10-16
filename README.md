# Tensorslow

This is a mini-implementation of a Tensorflow's eager API. It is really inefficient (hence the name, ha ha) because of a lack of out-of-the-box vectorization support in C#, but I tried to explore the workings of automatic differentiation in practice by manually specifying the operator overloads. 
