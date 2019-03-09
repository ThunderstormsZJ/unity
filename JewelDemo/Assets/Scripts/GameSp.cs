using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 方块的节点脚本
public class GameSp : MonoBehaviour {
    private GameModel.SpType type;
    private GameModel.StyleType styleType;
    private GameManager manager;
    private int row;
    private int col;
    private SpMove moveComponent;

    public int Col
    {
        get
        {
            return col;
        }

        set
        {
            col = value;
        }
    }
    public int Row
    {
        get
        {
            return row;
        }

        set
        {
            row = value;
        }
    }
    public GameModel.SpType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }
    public GameModel.StyleType StyleType
    {
        get
        {
            return styleType;
        }

        set
        {
            styleType = value;
        }
    }
    public GameManager Manager
    {
        get
        {
            return manager;
        }

        set
        {
            manager = value;
        }
    }
    public SpMove MoveComponent
    {
        get
        {
            return moveComponent;
        }
    }

    public void init(int _col, int _row, GameModel.SpType _type, GameManager _manager)
    {
        col = _col;
        row = _row;
        type = _type;
        Manager = _manager;
        // 随机生成样式
        genStyle();
    }

    public void updateColAndRow(int _row, int _col)
    {
        row = _row;
        col = _col;
    }

    // 是否可以移动
    public bool canMove()
    {
        return moveComponent != null;
    }

    // 判断other是否相邻
    public bool isFriend(GameObject other)
    {
        GameSp otherSp = other.GetComponent<GameSp>();
        if (otherSp == null) return false;

        return (otherSp.Row == Row && Mathf.Abs(otherSp.Col - Col) == 1) || (otherSp.Col == Col && Mathf.Abs(otherSp.Row - Row) == 1);
    }

    public void clear()
    {
        StartCoroutine(clearCoroutine());
    }

    private IEnumerator clearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play("SpDestory");
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }
    }

    private void genStyle()
    {
        SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
        GameModel.StyleSprite? randomStyleSprite = GameModel.Instance.getRandomStyleByType(type);
        if (randomStyleSprite != null)
        {
            styleType = randomStyleSprite.Value.type;
            spriteRender.sprite = randomStyleSprite.Value.sprite;
        }
    }

    private void Awake()
    {
        moveComponent = GetComponent<SpMove>();    
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
