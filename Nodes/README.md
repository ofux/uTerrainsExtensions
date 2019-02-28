# Nodes

In this directory (and subdirectories), you will find nodes which can be used with the 
node-based editor of the generation flow (biome and biome selector configuration).

A node can have many inputs, but only one output. The input and output values of a node are always of type `double`.

## Combiners

Combiners node combine the outputs of other nodes in different ways. For example, you may use them to add, subsctract, multiply or divide node outputs between each others.

## Filters

Filters transform the output of another node in different ways. For example, it can scale the output of another node, or return its absolute value, etc.

## Primitives

Primitives generate values. It doesn't have any input. For example, Perlin Noise is a primitive.

## Transformers

Transformers are a bit like filters as they transform the output of another node in different ways. Transformers are generally more complex than filters. Their goal is not just to change the output of another node, but is more about giving it another meaning. For example, the "To Height" transformer changes its input value into something that represents the vertical distance between the voxel and the terrain height.
