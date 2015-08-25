using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeler.Data.Scene;

namespace Modeler.Undo
{
    class UndoStack
    {
        public class Element
        {
            public Element next;
            public Scene scene;
        }

        public Element first = null;
        public Element firstRedo = null;
        private int MAX_MEMORY = 10000000; //maksymalne zużycie pamięci na stos cofania (w bajtach)

        public void Save(Scene scene)
        {
            //zapisanie obecnego stanu
            Element second = first;
            first = new Element();
            first.next = second;
            first.scene = new Scene(scene);

            //funkcja "powtórz" jest dostępna tylko jeśli ostatnią czynnością było cofnięcie
            firstRedo = null;
            int m = 0;
            for (Element el=first;el!=null;el=el.next)
            {
                m+=el.scene.estimatedMemory();
                if (m > MAX_MEMORY) el.next=null;
            }
        }

        public Scene Undo(Scene scene)
        {
            if (first != null)
            {
                Element secondRedo = firstRedo;
                firstRedo = new Element();
                firstRedo.next = secondRedo;
                firstRedo.scene = new Scene(scene);

                scene =  first.scene;
                first = first.next;
            }
            return scene;
        }

        public Scene Redo(Scene scene)
        {
            if (firstRedo != null)
            {
                Element second = first;
                first = new Element();
                first.next = second;
                first.scene = new Scene(scene);

                scene = firstRedo.scene;
                firstRedo = firstRedo.next;
            }
            return scene;
        }
    }
}
