﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunt;
using Lunt.Diagnostics;
using Lunt.IO;
using Surface.Pipeline.Content;

namespace Surface.Pipeline.Writers
{
    public sealed class MapWriter : Writer<MapContent>
    {
        public override void Write(Context context, IFile target, MapContent value)
        {
            using (var stream = target.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream))
            {
                // Write basic map information.
                writer.Write(value.Size.Width);
                writer.Write(value.Size.Height);

                // Write tilesets.
                writer.Write(value.Tilesets.Count);
                foreach (TilesetReference tileset in value.Tilesets)
                {
                    writer.Write(tileset.Index);
                    writer.Write(tileset.Asset.FullPath);
                }

                // Write tile layer information.
                writer.Write(value.Layers.Count);
                foreach (var layer in value.Layers)
                {
                    WriteTileLayer(writer, layer);
                }
            }
        }

        private static void WriteTileLayer(BinaryWriter writer, TileLayerContent layer)
        {
            // Write layer information.
            writer.Write(layer.Id);
            writer.Write(layer.Name);
            writer.Write(layer.Opacity);

            // Write all tiles.
            writer.Write(layer.Tiles.Count);
            foreach (var tile in layer.Tiles)
            {
                writer.Write7BitEncodedInteger(tile.TileSetId);
                writer.Write7BitEncodedInteger(tile.TilesetIndex);
                writer.Write7BitEncodedInteger(tile.GridIndex);
            }

            // Write all entities.
            writer.Write(layer.Entities.Count);
            foreach (var entity in layer.Entities)
            {
                writer.Write(entity.Name);
                writer.Write(entity.Type);
                writer.Write(entity.Position.X);
                writer.Write(entity.Position.Y);
                writer.Write(entity.Size.Width);
                writer.Write(entity.Size.Height);

                // Write properties.
                writer.Write(entity.Properties.Count);
                foreach (var property in entity.Properties)
                {
                    writer.Write(property.Name);
                    writer.Write(property.Value);
                }
            }
        }
    }
}
